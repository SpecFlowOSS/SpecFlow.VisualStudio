using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.IdeIntegration.Bindings;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.LanguageService;
using TechTalk.SpecFlow.VsIntegration.Utils;

namespace TechTalk.SpecFlow.VsIntegration.Bindings.Discovery
{
    public class VsBindingRegistryBuilder
    {
        private readonly IIdeTracer tracer;
        private readonly VsBindingReflectionFactory bindingReflectionFactory = new VsBindingReflectionFactory();

        public VsBindingRegistryBuilder(IIdeTracer tracer)
        {
            this.tracer = tracer;
        }

        public IEnumerable<IStepDefinitionBinding> GetBindingsFromProjectItem(ProjectItem projectItem)
        {
            List<ProjectItem> relatedProjectItems;
            return GetBindingsFromProjectItem(projectItem, out relatedProjectItems);
        }

        public IEnumerable<IStepDefinitionBinding> GetBindingsFromProjectItem(ProjectItem projectItem, out List<ProjectItem> relatedProjectItems)
        {
            var bindingProcessor = new IdeBindingSourceProcessor(tracer);
            relatedProjectItems = new List<ProjectItem>();
            ProcessBindingsFromProjectItem(projectItem, bindingProcessor, relatedProjectItems);
            return bindingProcessor.ReadStepDefinitionBindings();
        }

        private void ProcessBindingsFromProjectItem(ProjectItem projectItem, IdeBindingSourceProcessor bindingSourceProcessor, List<ProjectItem> relatedProjectItems)
        {
            foreach (CodeClass codeClass in VsxHelper.GetClasses(projectItem))
            {
                CodeClass2 bindingClassIncludingParts = codeClass as CodeClass2;

                var parts = new List<CodeClass>();

                if (bindingClassIncludingParts == null)
                {
                    parts.Add(codeClass);
                }
                else
                {
                    parts.AddRange(bindingClassIncludingParts.Parts.OfType<CodeClass>());
                }


                var baseClass = codeClass.Bases.OfType<CodeClass>().FirstOrDefault();

                while (baseClass != null && baseClass.FullName != "System.Object")
                {
                    tracer.Trace("Adding inherited bindings for class: " + baseClass.FullName, GetType().Name);
                    parts.Add(baseClass);
                    baseClass = baseClass.Bases.OfType<CodeClass>().FirstOrDefault();
                }

                // we need to use the class parts to grab class-related information (e.g. [Binding] attribute)
                // but we need to process the binding methods only from the current part, otherwise these
                // methods would be registered to multiple file paths, and the update tracking would not work
                    
                relatedProjectItems.AddRange(parts.Select(p => p.ProjectItem).Where(pi => pi != null && pi != projectItem));
                ProcessCodeClass(codeClass, bindingSourceProcessor, parts.ToArray());
            }
        }

        private void ProcessCodeClass(CodeClass codeClass, IdeBindingSourceProcessor bindingSourceProcessor, params CodeClass[] classParts)
        {
            
            var filteredAttributes = classParts
                .SelectMany(cc => cc.Attributes.Cast<CodeAttribute2>().Where(attr => CanProcessTypeAttribute(bindingSourceProcessor, attr))).ToArray();

            if (!bindingSourceProcessor.PreFilterType(filteredAttributes.Select(attr => attr.FullName)))
                return;
        
            
            var bindingSourceType = bindingReflectionFactory.CreateBindingSourceType(classParts, filteredAttributes); //TODO: merge info from parts

            if (!bindingSourceProcessor.ProcessType(bindingSourceType))
                return;

            ProcessCodeFunctions(codeClass, bindingSourceType, bindingSourceProcessor);

            bindingSourceProcessor.ProcessTypeDone();
            // process base classes
            foreach (var part in classParts.Where(cp=> cp != codeClass))
            {
                var bindingSourceType2 = bindingReflectionFactory.CreateBindingSourceType(new[] { part }, filteredAttributes);  

                if (!bindingSourceProcessor.ProcessType(bindingSourceType2))
                    return;

                ProcessCodeFunctions(part, bindingSourceType2, bindingSourceProcessor);

                bindingSourceProcessor.ProcessTypeDone();
            }
        }

        private void ProcessCodeFunctions(CodeClass codeClass, BindingSourceType bindingSourceType, IdeBindingSourceProcessor bindingSourceProcessor)
        {
            foreach (var codeFunction in codeClass.Children.OfType<CodeFunction>())
            {
                var bindingSourceMethod = CreateBindingSourceMethod(codeFunction, bindingSourceType, bindingSourceProcessor);
                if (bindingSourceMethod != null)
                    bindingSourceProcessor.ProcessMethod(bindingSourceMethod);
            }
        }

        private static bool CanProcessTypeAttribute(IdeBindingSourceProcessor bindingSourceProcessor, CodeAttribute2 attr)
        {
            string attributeTypeName;
            try
            {
                attributeTypeName = attr.FullName;
            }
            catch (Exception)
            {
                // invalid attribute - ignore
                return false;
            }

            return bindingSourceProcessor.CanProcessTypeAttribute(attributeTypeName);
        }

        private BindingSourceMethod CreateBindingSourceMethod(CodeFunction codeFunction, BindingSourceType bindingSourceType, IdeBindingSourceProcessor bindingSourceProcessor)
        {
            try
            {
                var filteredAttributes = codeFunction.Attributes.Cast<CodeAttribute2>().Where(attr => bindingSourceProcessor.CanProcessTypeAttribute(attr.FullName)).ToArray();
                return bindingReflectionFactory.CreateBindingSourceMethod(codeFunction, bindingSourceType, filteredAttributes);
            }
            catch (Exception ex)
            {
                tracer.Trace("CreateBindingSourceMethod error: {0}", this, ex);
                return null;
            }
        }
    }
}
