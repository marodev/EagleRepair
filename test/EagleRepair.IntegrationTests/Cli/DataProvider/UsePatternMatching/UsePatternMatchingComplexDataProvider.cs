using System.Collections.Generic;

namespace EagleRepair.IntegrationTests.Cli.DataProvider.UsePatternMatching
{
    public static class UsePatternMatchingComplexDataProvider
    {
        private const string Input = @"
#define pi
namespace Entry
{
    public class C
    {
        public interface IUser {}
        public class User : IUser
        {
            public int Count;
        }
        
        public class Event
        {
            public IUser Entity { get; set; }
        }
        
        public delegate void Notify(int i);  // delegate
        public event Notify ProcessCompleted;
        
        public void M(Event e)
        {
            ProcessCompleted += i =>
            {
#if (pi)
                var user = e.Entity as User;
                if (user != null)
                {
                    user.Count = i;
                }
#endif
            };
            
            ProcessCompleted += i =>
            {
                var user = e.Entity as User;
                if (user != null)
                {
                    user.Count = i;
                }
            };
        }
    }
}";

        private const string ExpectedOutput = @"
#define pi
namespace Entry
{
    public class C
    {
        public interface IUser {}
        public class User : IUser
        {
            public int Count;
        }
        
        public class Event
        {
            public IUser Entity { get; set; }
        }
        
        public delegate void Notify(int i);  // delegate
        public event Notify ProcessCompleted;
        
        public void M(Event e)
        {
            ProcessCompleted += i =>
            {
#if (pi)
                if (e.Entity is User user)
                {
                    user.Count = i;
                }
#endif
            };
            
            ProcessCompleted += i =>
            {
                if (e.Entity is User user)
                {
                    user.Count = i;
                }
            };
        }
    }
}";

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { Input, ExpectedOutput };
        }
    }
}
