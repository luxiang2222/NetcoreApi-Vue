using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetCoreWithVue.Model
{
  public class LogOperationModel
  {
    public string username { get; set; }
    public string hostname { get; set; }
    public string ipaddress { get; set; }
    public DateTime operatetime { get; set; }
    public string mode { get; set; }
    public string content { get; set; }
    public string oldvalue { get; set; }
    public string newvalue { get; set; }
    public string remark { get; set; }
    public string level { get; set; }
  }
}
