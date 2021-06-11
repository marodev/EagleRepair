# Examples

The following examples show the source-code <em>before</em> (Non-Compliant) and <em>after</em>,
containing the EagleRepair's fix (Compliant).

!!! example "R1 DisposePattern"

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


!!! example "R2 MergeSequentialChecks"

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


!!! example "R3 NullChecksShouldNotBeUsedWithIs"

    Non-Compliant
    ```c# hl_lines="3"
    public void M(object s)
    {
        if (s != null && s is string)
        {
            // do something
        }
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="3"
    public void M(object s)
    {
        if (s is string)
        {
            // do something
        }
    }
    ```

!!! example "R4 SimplifyLinq"

    Non-Compliant
    ```c#
    public void M()
    {
        var list = new List<int>();
        var result1 = list.Where(i => i is not null && i.ToString().Equals("foo")).Count();
        var result2 = list.Where(i => i > 0).First();
        var result3 = list.Where(i => i > 0).FirstOrDefault();
        var result4 = list.Where(i => i > 0).Single();
        var result5 = list.Where(i => i > 0).SingleOrDefault();
        var result6 = list.Where(i => i is not null && i.ToString().Equals("foo")).Last();
        var result7 = list.Where(element => element is T).Select(e => e as T);
    }
    ```
    
    Compliant
    
    ``` c#
    public void M()
    {
        var list = new List<int>();
        var result1 = list.Count(i => i is not null);
        var result2 = list.First(i => i > 0);
        var result3 = list.FirstOrDefault(i => i > 0);
        var result4 = list.Single(i => i > 0);
        var result5 = list.SingleOrDefault(i => i > 0);
        var result6 = list.Last(i => i is not null);
        var result7 = list.OfType<T>();
    }
    ```


!!! example "R5 TypeCheckAndCast"

    Non-Compliant

    ```c# hl_lines="3"
    if (o is string)
    {
        var str = (string) o;
        var length = str.Length.GetHashCode();
        return length > 2;
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="1"
    if (o is string str)
    {
        var length = str.Length.GetHashCode();
        return length > 2;
    }
    ```


!!! example "R6 UseMethodAny"

    Non-Compliant

    ```c# hl_lines="1"
    if (list.Count() > 0)
    {
        Console.WriteLine("List is not empty.");
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="1"
    if (list.Any())
    {
        Console.WriteLine("List is not empty.");
    }
    ```

!!! example "R7 UseNullPropagation"

    Non-Compliant

    ```c# hl_lines="3-6"
    public void M(Car c)
    {
        if (c != null)
        {
            c.Fly(42);
        }
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="3"
    public void M(Car c)
    {
        c?.Fly(42);
    }
    ```


!!! example "R8 UsePatternMatching"

    Non-Compliant

    ```c# hl_lines="3 5"
    public void M(object o)
    {
        var s = o as string;
        // Check if string is not null
        if (s != null) {
            Console.WriteLine($"Hi. {s}");
        }
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="4"
    public void M(object o)
    {
        // Check if string is not null
        if (o is string s) {
            Console.WriteLine($"Hi. {s}");
        }
    }
    ```

!!! example "R9 UseStringInterpolation"

    Non-Compliant

    ```c# hl_lines="6"
    public IList<string> M()
    {
        const string msg = "Value is:";
        return new List<string>
            {
                string.Format("{0} {1}", msg, _utils.GetValue())
            };
        }
    }
    ```
    
    Compliant
    
    ```c# hl_lines="6"
    public IList<string> M()
    {
        const string msg = "Value is:";
        return new List<string>
            {
                $"{msg} {_utils.GetValue()}"
            };
        }
    }
    ```

!!! example "R10 UseStringIsNullOrEmpty"

    Non-Compliant

    ```c# hl_lines="3"
    public void M(string s)
    {
        if(s != null && s.Length > 0) {
            // do something
        }
    }
    ```
    
    Compliant
    
    ``` c# hl_lines="3"
    public void M(string s)
    {
        if(!string.IsNullOrEmpty(s)) {
            // do something
        }
    }
    ```



