
using System;
using System.Collections.Generic;
using System.Linq;
using JPMorrow.Revit.Documents;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.Test
{
    public class TestResult
    {
        public string Message { get; private set; }
        public bool Passed { get; private set; }
        public string Result { get => Passed ? "Passed" : "Failed"; }

        public TestResult(string message, bool passed)
        {
            Message = message;
            Passed = passed;
        }

        public static string PrintAllTests(IEnumerable<TestResult> results)
        {
            results = results.OrderBy(x => x.Message);
            string o = "";
            
            foreach(var r in results)
            {
                o += r.Message + " -> " + r.Result + "\n";
            }

            return o;
        }
    }
    
    public static partial class TestBed
    {
        public static void TestAll(ModelInfo info)
        {
            var methods = typeof(TestBed).GetMethods().Where(x => !x.Name.Contains("TestAll"));
            List<TestResult> results = new List<TestResult>();

            foreach(var m in methods)
            { 
                if(m.Name.ToLower().StartsWith("test"))
                {
                    var result = m.Invoke(null, new object[] { ModelInfo.SettingsBasePath, info.DOC, info.UIDOC });
                    results.Add(result as TestResult);
                }
            }

            debugger.show(header:"BOM", err:TestResult.PrintAllTests(results));
        }
    }
}