using NUnit.Framework;
using System;
using System.IO;


// http://www.alteridem.net/2016/10/03/nunit-unit-tests/

namespace JIRASupport.Test
{
    [TestFixture]
    public class DummyTest
    {
        private const string expected = "Test Run";

        [Test]
        public void RunTest()
        {
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);

                JIRASupport.MainForm.RunTest();

                var result = sw.ToString().Trim();

                Assert.AreEqual(expected, result);
            }
        }
    }
}
