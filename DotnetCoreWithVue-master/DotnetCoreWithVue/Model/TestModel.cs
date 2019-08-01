using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetCoreWithVue.Model
{
  public class TestModel
  {
    public Guid TaskId{ get; set; }
    public string LoginName { get; set; }
    public DateTime ImportTime { get; set; }
    public string DoType { get; set; }
    public int Count { get; set; }
  }
}
