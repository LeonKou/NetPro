namespace NetPro.Dapper.Parameters
{
    /// <summary>
    /// sql条件操作类型
    /// </summary>
    public enum OperateType
    {
        Equal,
        NotEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,

        Like,
        LeftLike,
        RightLike,

        NotLike,
        In,
        NotIn,

        SqlFormat,

        SqlFormatPar,

        Between,
        End
    }
}
