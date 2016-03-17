using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Core;
using NUnit.Core.Extensibility;
using NUnit.Framework;

namespace Audi.UnitTests
{
    [TestFixture]
    public class NUnit
    {
        [Test]
        public void IAmAFailingTest()
        {
            A500.Setup();
            Assert.IsTrue(false);
        }
    }
}