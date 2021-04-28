namespace EagleRepair.Cli.Input
{
    public interface ICmdLineValidator
    {
        public CmdOptions Validate(CmdOptions options);
    }
}
