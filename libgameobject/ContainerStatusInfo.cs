using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace libgameobject
{
    public class ContainerStatusInfo
    {
        /// <summary>
        /// Заполненость контейнера при вывозе
        /// </summary>
        public List<double> l1 { set; get; } = new List<double>();
        /// <summary>
        /// Штрафы
        /// </summary>
        public List<double> l2 { set; get; } = new List<double>();
        /// <summary>
        /// Средняя заполненость контейнера при вывозе
        /// </summary>
        [Display(Name = "Средняя заполненость контейнера при вывозе")]
        public double t1 => l1.Count > 0 ? l1.Average() : 0;//l1.Count > 0 ? l1.Sum() / l1.Count : 0;
        /// <summary>
        /// Количество вывозов
        /// </summary>
        [Display(Name = "Количество вывозов")]
        public double t2 => l1.Count;
        /// <summary>
        /// Количество переполнений
        /// </summary>
        [Display(Name = "Количество переполнений")]
        public double t3 { set; get; } = 0;
        /// <summary>
        /// Максимальный штраф
        /// </summary>
        [Display(Name = "Максимальный штраф")]
        public double t4 => l2.Count > 0 ? l2.Max() : 0;
        /// <summary>
        /// Средний штраф
        /// </summary>
        [Display(Name = "Средний штраф")]
        public double t5 => l2.Count > 0 ? l2.Average() : 0;
    }
}
