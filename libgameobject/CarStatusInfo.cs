using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace libgameobject
{
    public class CarStatusInfo
    {
        private Car _car;
        public CarStatusInfo(Car car)
        {
            _car = car;
        }
        /// <summary>
        /// Перевезенный груз
        /// </summary>
        public List<double> l1 { set; get; } = new List<double>();
        /// <summary>
        /// Заправки
        /// </summary>
        public List<double> l2 { set; get; } = new List<double>();
        /// <summary>
        /// Ремонты
        /// </summary>
        public List<double> l3 { set; get; } = new List<double>();
        /// <summary>
        /// Время в пути
        /// </summary>
        [Display(Name = "Время в пути")]
        public double t1 { set; get; } = 0;
        /// <summary>
        /// Пройденное расстояние
        /// </summary>
        [Display(Name = "Пройденное расстояние")]
        public double t2 { set; get; } = 0;
        /// <summary>
        /// Заправленое топливо
        /// </summary>
        [Display(Name = "Заправленое топливо")]
        public double t3 => l2.Sum();
        /// <summary>
        /// Накопленный износ
        /// </summary>
        [Display(Name = "Накопленный износ")]
        public double t4 => l3.Sum()+_car.Wearout;
        /// <summary>
        /// Перевезенный груз
        /// </summary>
        [Display(Name = "Перевезенный груз")]
        public double t5 => l1.Sum();
        /// <summary>
        /// Количество заправок
        /// </summary>
        [Display(Name = "Количество заправок")]
        public double t6 => l2.Count;
        /// <summary>
        /// Средний объем перевезенного груза
        /// </summary>
        [Display(Name = "Средний объем перевезенного груза")]
        public double t7 => l1.Count > 0 ? l1.Average() : 0;
        /// <summary>
        /// Среднее количество заправляемого топлива
        /// </summary>
        [Display(Name = "Среднее количество заправляемого топлива")]
        public double t8 => l2.Count > 0 ? l2.Average() : 0;
        /// <summary>
        /// Количество ремонтов
        /// </summary>
        [Display(Name = "Количество ремонтов")]
        public double t9 => l3.Count;
        /// <summary>
        /// Средний показатель износа при ремонте
        /// </summary>
        [Display(Name = "Средний показатель износа при ремонте")]
        public double t10 => l3.Count > 0 ? l3.Average() : 0;
        /// <summary>
        /// Количество поломок
        /// </summary>
        [Display(Name = "Количество поломок")]
        public double t11 { set; get; } = 0;
        /// <summary>
        /// Количество вывезеных контейнеров
        /// </summary>
        [Display(Name = "Количество вывезеных контейнеров")]
        public double t12 { set; get; } = 0;
        /// <summary>
        /// Возвратов на станцию
        /// </summary>
        [Display(Name = "Возвратов на станцию")]
        public double t13 { set; get; } = 0;
    }
}
