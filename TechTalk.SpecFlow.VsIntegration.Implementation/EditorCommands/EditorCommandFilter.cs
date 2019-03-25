﻿using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Implementation.Commands;

namespace TechTalk.SpecFlow.VsIntegration.Implementation.EditorCommands
{
    public class EditorCommandFilter
    {
// ReSharper disable NotAccessedField.Local
        private readonly IIdeTracer tracer;
// ReSharper restore NotAccessedField.Local
        private readonly IGoToStepDefinitionCommand goToStepDefinitionCommand;

        private readonly FormatTableCommand formatTableCommand;
        private readonly CommentUncommentCommand commentUncommentCommand;
        private readonly RenameCommand renameCommand;

        public EditorCommandFilter(IIdeTracer tracer, IGoToStepDefinitionCommand goToStepDefinitionCommand, FormatTableCommand formatTableCommand, CommentUncommentCommand commentUncommentCommand, RenameCommand renameCommand)
        {
            this.goToStepDefinitionCommand = goToStepDefinitionCommand;
            this.formatTableCommand = formatTableCommand;
            this.commentUncommentCommand = commentUncommentCommand;
            this.renameCommand = renameCommand;
            this.tracer = tracer;
        }

        private char GetTypeChar(IntPtr pvaIn)
        {
            return (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
        }

        public bool QueryStatus(GherkinEditorContext editorContext, Guid pguidCmdGroup, OLECMD prgCmd)
        {
            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                var vsStd97CmdId = (VSConstants.VSStd97CmdID)prgCmd.cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/VSStd97CmdID:{0}", this, vsStd97CmdId);
#endif
                switch (vsStd97CmdId)
                {
                    case VSConstants.VSStd97CmdID.GotoDefn:
                        if (goToStepDefinitionCommand.CanGoToDefinition(editorContext))
                            return true;
                        break;
                }
            }
            else if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                var vsStd2KCmdId = (VSConstants.VSStd2KCmdID)prgCmd.cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/VSStd2KCmdID:{0}", this, vsStd2KCmdId);
#endif
                switch (vsStd2KCmdId)
                {
                    case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                    case VSConstants.VSStd2KCmdID.RENAME:
                        return true;
                }
            }
            else if (pguidCmdGroup == GuidList.guidSpecFlowCmdSet)
            {
                var specFlowCmdSet = (SpecFlowCmdSet)prgCmd.cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/SpecFlowCmdSet:{0}", this, specFlowCmdSet);
#endif
                switch (specFlowCmdSet)
                {
                    case SpecFlowCmdSet.RunScenarios:
                    case SpecFlowCmdSet.DebugScenarios:
                        return true;
                }
            }
            else if(pguidCmdGroup == ReSharperCommandGroups.CommandGroup)
            {
                var reSharperCmd = (ReSharperCommand)prgCmd.cmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("QueryStatus/ReSharperCommand:{0}", this, reSharperCmd);
#endif
                switch (reSharperCmd)
                {
                    case ReSharperCommand.GotoDeclaration:
                        if (goToStepDefinitionCommand.CanGoToDefinition(editorContext))
                            return true;
                        break;
                    case ReSharperCommand.LineComment:
                        return true;
                    case ReSharperCommand.UnitTestRunContext:
                    case ReSharperCommand.UnitTestDebugContext:
                        return true;
                }
            }
#if TRACE_VS_COMMANDS
            else
            {
                tracer.Trace("QueryStatus/Other:{0} / {1}", this, pguidCmdGroup, prgCmd.cmdID);
            }
#endif

            return false;
        }

        public bool PreExec(GherkinEditorContext editorContext, Guid pguidCmdGroup, uint nCmdID)
        {
            if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                var vsStd97CmdId = (VSConstants.VSStd97CmdID)nCmdID;
#if TRACE_VS_COMMANDS
                if (vsStd97CmdId != VSConstants.VSStd97CmdID.SearchCombo && vsStd97CmdId != VSConstants.VSStd97CmdID.SolutionCfg)
                    tracer.Trace("Exec/VSStd97CmdID:{0}", this, vsStd97CmdId);
#endif
                switch (vsStd97CmdId)
                {
                    case VSConstants.VSStd97CmdID.GotoDefn:
                        if (goToStepDefinitionCommand.GoToDefinition(editorContext))
                            return true;
                        break;
                }
            }
            else if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                var vsStd2KCmdId = (VSConstants.VSStd2KCmdID)nCmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/VSStd2KCmdID:{0}", this, vsStd2KCmdId);
#endif
                switch (vsStd2KCmdId)
                {
                    case VSConstants.VSStd2KCmdID.COMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.COMMENTBLOCK:
                        if (commentUncommentCommand.CommentOrUncommentSelection(editorContext, CommentUncommentAction.Comment))
                            return true;
                        break;
                    case VSConstants.VSStd2KCmdID.UNCOMMENT_BLOCK:
                    case VSConstants.VSStd2KCmdID.UNCOMMENTBLOCK:
                        if (commentUncommentCommand.CommentOrUncommentSelection(editorContext, CommentUncommentAction.Uncomment))
                            return true;
                        break;
                    case VSConstants.VSStd2KCmdID.RENAME:
                        if (renameCommand.Rename(editorContext))
                            return true;
                        break;
                }
            }
            else if (pguidCmdGroup == GuidList.guidSpecFlowCmdSet)
            {
                var specFlowCmdSet = (SpecFlowCmdSet)nCmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/SpecFlowCmdSet:{0}", this, specFlowCmdSet);
#endif
            }
            else if(pguidCmdGroup == ReSharperCommandGroups.CommandGroup)
            {
                var reSharperCmd = (ReSharperCommand)nCmdID;
#if TRACE_VS_COMMANDS
                tracer.Trace("Exec/ReSharperCommand:{0}", this, reSharperCmd);
#endif
                switch (reSharperCmd)
                {
                    case ReSharperCommand.GotoDeclaration:
                        if (goToStepDefinitionCommand.GoToDefinition(editorContext))
                            return true;
                        break;
                    case ReSharperCommand.LineComment:
                        if (commentUncommentCommand.CommentOrUncommentSelection(editorContext, CommentUncommentAction.Toggle))
                            return true;
                        break;
                }
            }
#if TRACE_VS_COMMANDS
            else
            {
                tracer.Trace("Exec/Other:{0} / {1}", this, pguidCmdGroup, nCmdID);
            }
#endif

            return false;
        }

        public void PostExec(GherkinEditorContext editorContext, Guid pguidCmdGroup, uint nCmdID, IntPtr pvaIn)
        {
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch ((VSConstants.VSStd2KCmdID) nCmdID)
                {
                    case VSConstants.VSStd2KCmdID.TYPECHAR:
                        var ch = GetTypeChar(pvaIn);
                        if (ch == '|')
                            formatTableCommand.FormatTable(editorContext);
                        break;
                }
            }
//uncomment this to add further command handlers
//                if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
//                {
//                    switch ((VSConstants.VSStd97CmdID)nCmdID)
//                    {
//                    }
//                }
        }
    }

}