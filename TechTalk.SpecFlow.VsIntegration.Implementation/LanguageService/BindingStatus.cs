namespace TechTalk.SpecFlow.VsIntegration.Implementation.LanguageService
{
    public class BindingStatus
    {
        public readonly static BindingStatus UnknownBindingStatus = new BindingStatus(BindingStatusKind.Unknown);
        public readonly static BindingStatus UnboundBindingStatus = new BindingStatus(BindingStatusKind.Unbound);

        public BindingStatusKind Kind { get; private set; }

        public BindingStatus(BindingStatusKind kind)
        {
            Kind = kind;
        }
    }
}