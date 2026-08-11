namespace QueryBuilderWebApplication.Abstractions;

public interface IDelete
{
    Task<bool> DeleteAsync(int studentNumber);
}