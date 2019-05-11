using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace libgameobject
{
    public class SimulationInfo
    {
        /// <summary>
        /// Монет потрачено
        /// </summary>
        [Display(Name = "Монет потрачено")]
        public double t1 { set; get; } = 0;
        /// <summary>
        /// Монет получено
        /// </summary>
        [Display(Name = "Монет получено")]
        public double t2 { set; get; } = 0;
        
    }
}
