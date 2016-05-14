using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CisLab3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Information info = new Information();
            info.queryOne();
        }
    }

    public class Car
    {
        private string model;
        private int year;
        private Engine engine;

        public string Model 
        { 
            get { return this.model; } 
            set { this.model = value; } 
        }

        public int Year
        {
            get { return this.year; }
            set { this.year = value; }
        }

        public Engine Engine
        {
            get { return this.engine;  }
            set { this.engine = value;  }
        }


        public Car(string model, Engine engine, int year) 
        {
            this.model = model;
            this.engine = engine;
            this.year = year;
        }
    }

    public class Engine
    {
        private double displacement;
        private double horsePower;
        private string model;

        public double Displacement
        {
            get { return this.displacement; }
            set { this.displacement = value; }
        }

        public double HorsePower
        {
            get { return this.horsePower; }
            set { this.horsePower = value; }
        }

        public string Model
        {
            get { return this.model; }
            set { this.model = value; }
        }

        public Engine(double displacement, double power, string model)
        {
            this.displacement = displacement;
            this.horsePower = power;
            this.model = model;
        }

    }

    public class Information
    {
        List<Car> myCars = new List<Car>()
        {
            new Car("E250", new Engine(1.8, 204, "CGI"), 2009),
            new Car("E350", new Engine(3.5, 292, "CGI"), 2009),
            new Car("A6", new Engine(2.5, 187, "FSI"), 2012),
            new Car("A6", new Engine(2.8, 220, "FSI"), 2012),
            new Car("A6", new Engine(3.0, 295, "TFSI"), 2012),
            new Car("A6", new Engine(2.0, 175, "TDI"), 2011),
            new Car("A6", new Engine(3.0, 309, "TDI"), 2011),
            new Car("S6", new Engine(4.0, 414, "TFSI"), 2012),
            new Car("S8", new Engine(4.0, 513, "TFSI"), 2012)
         };

        public Information() { }

        public void queryOne() 
        {
            var result = from e in myCars
                         where e.Model == "A6"
                         select new { engineType = e.Engine.Model == "TDI" ? "disel": "petrol", hppl = e.Engine.HorsePower/e.Engine.Displacement };

            var result2 = from f in result
                          group f.hppl by f.engineType;

 
            foreach (var value in result)
            {
                Console.WriteLine(value.engineType);
            }

            foreach (var value2 in result2)
            {
                Console.WriteLine(value2);
            }

        }

    }
}
