using Func.Meta;

namespace FuncTest;
using static MetaEx;

public class MetaTest
{
    record TestFuncEnum : FuncEnum
    {
        public static readonly TestFuncEnum TT = new(1);
        public static readonly TestFuncEnum TT1 = new(2);
        public static readonly TestFuncEnum TT2 = new(3);
        public static readonly TestFuncEnum Test = new(4);

        protected TestFuncEnum(int original) : base(original) { }


    }

    [Fact]
    public void Meta_property_set_and_get()
    {
        T t = new("test", 1);

        Assert.Equal(2, t.SetMeta(TestFuncEnum.TT, 2).GetMeta<int>(TestFuncEnum.TT));

        // Add new value
        t.SetMeta(TestFuncEnum.TT, 5);
        Assert.Equal(5, t.GetMeta<int>(TestFuncEnum.TT));

        // Add new type value
        t.SetMeta(TestFuncEnum.TT, "Df");
        Assert.Equal("Df", t.GetMeta<string>(TestFuncEnum.TT));

        // Add second propery
        t.SetMeta(TestFuncEnum.TT1, 2);
        Assert.Equal(2, t.GetMeta<int>(TestFuncEnum.TT1));
        Assert.Equal("Df", t.GetMeta<string>(TestFuncEnum.TT));

        // Add third propery
        t.SetMeta(TestFuncEnum.TT2, true);
        Assert.True(t.GetMeta<bool>(TestFuncEnum.TT2));
        Assert.False(t.GetMeta<bool>(TestFuncEnum.TT));

    }

    [Fact]
    public void Meta_property_set_and_get_error()
    {
        T t = new("test", 1);
        S s = new();

        t.SetMeta(TestFuncEnum.Test, 2);
        Assert.Null(t.GetMeta<string>(TestFuncEnum.TT));
        Assert.Equal(0, t.GetMeta<int>(TestFuncEnum.TT));
        Assert.Equal(0.0, t.GetMeta<double>(TestFuncEnum.TT));
        Assert.NotEqual(s, t.GetMeta<S>(TestFuncEnum.TT));
        Assert.Equal(AB.a, t.GetMeta<AB>(TestFuncEnum.TT));
    }

    [Fact]
    public void Meta_property_comparer()
    {
        var v1 = TestFuncEnum.TT;
        var v2 = TestFuncEnum.TT;

        Assert.Equal(v1, v2);
    }

    class T(string s, int i)
    {
        string StringProperty { get; set; } = s;
        int IntProperty { get; set; } = i;
    }
    struct S() { public double d = 10; }
    enum AB { a, b }
}
