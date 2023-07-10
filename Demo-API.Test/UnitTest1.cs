using Microsoft.AspNetCore.Http;
using Demo_API.Types;

namespace Demo_API.Test;

[TestClass]
public class UnitTest1
{

    private const string TestString = "FunFact1";

    [TestMethod]
    public void TestMethod1()
    {
        DemoEntity demoEntity = new DemoEntity(TestString);
        Assert.AreEqual(demoEntity.FunFact, TestString);
    }
}
