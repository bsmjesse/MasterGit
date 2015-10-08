using System;

namespace VLF.DAS.Logic
{
  public abstract class ActiveRecord :Das
  {
    public abstract string Get(int pageSize, int pageNumber, params string[] fieldNames);

    protected ActiveRecord(string connectionString) : base(connectionString)
    {
    }
  }
}
