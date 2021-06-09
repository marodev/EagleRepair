# Examples

Coming soon!

???+ R1

    Non-Compliant
    ```c# hl_lines="9"
    using System;
    
    namespace N
    {
        public class C : IDisposable
        {
            public void Dispose()
            {
                // cleanup
            }
        }
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="25"
    using System;
    
    namespace N
    {
        public class C : IDisposable
        {
            // To detect redundant calls
            private bool _disposed = false;
    
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
    
            protected virtual void Dispose(bool disposing)
            {
                if (_disposed)
                {
                    return;
                }
    
                if (disposing)
                {
                    // cleanup
                }
    
                _disposed = true;
            }
        }
    }
    ```


??? R2

    Non-Compliant
    ```c# hl_lines="3"
    public void M(B b)
    {
        if (b == null || b.Parent is null)
        {
            // do something
        }
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="3"
    public void M(B b)
    {
        if (b?.Parent is null)
        {
            // do something
        }
    }
    ```



