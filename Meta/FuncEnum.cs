namespace Func.Meta;

public record FuncEnum
{
    public static readonly FuncEnum TypeDescriptionProvider = new(0);
/*    public static readonly FuncEnum IsProfile = new(1);
    public static readonly FuncEnum IsControl = new(2);
    public static readonly FuncEnum IsExpert = new(3);*/

    public int InternalValue { get; protected set; }

    protected FuncEnum(int internalValue)
    {
        this.InternalValue = internalValue;
    }
}

/*
public class MyEnum : MyBaseEnum
{
    public static readonly MyEnum D = new MyEnum( 4 );
    public static readonly MyEnum E = new MyEnum( 5 );

    protected MyEnum( int internalValue ) : base( internalValue )
    {
        // Nothing
    }
}

[TestMethod]
public void EnumTest()
{
    this.DoSomethingMeaningful( MyEnum.A );
}

private void DoSomethingMeaningful( MyBaseEnum enumValue )
{
    if( enumValue == MyEnum.A ) { ...}
    else if (enumValue == MyEnum.B) { ...}
}     
*/
