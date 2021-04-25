namespace Cli
{
    public interface ICmdLineValidator
    {
        public CmdOptions Validate(CmdOptions options);
    }
}