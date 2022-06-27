using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NasBI.Truenas
{
    public interface IUnmarshaller<TResult>
    {
        TResult Unmarshal(TruenasResponse value);
    }
}
