namespace EagleRepair.Statistics
{
    public enum SonarQubeRule
    {
        S1155, // ’Any()’ should be used to test for emptiness
        S2971, // IEnumerable LINQs should be simplified
        S3247, // Duplicate casts should not be made1
        S3256, // string.IsNullOrEmpty should be used
        S4201 // Null checks should not be used with ’is’
    }
}
