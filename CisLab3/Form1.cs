using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml;

namespace CisLab3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Information info = new Information();
            info.queryOne();
            info.serialization();
            info.createXmlFromLinq();
            info.modifyingXMLDocument();
            info.LINQtoXHTML();
            info.XPath();
        }
    }

    [XmlType("car")]
    [XmlRoot("car")]
    public class Car
    {
        [XmlElement(ElementName = "model")]
        private string model;
        [XmlElement(ElementName = "engine")]
        private Engine motor;
        [XmlElement(ElementName = "year")]
        private int year;

        public string Model 
        { 
            get { return this.model; } 
            set { this.model = value; } 
        }

        public Engine Engine
        {
            get { return this.motor; }
            set { this.motor = value; }
        }

        public int Year
        {
            get { return this.year; }
            set { this.year = value; }
        }

        public Car() { }

        public Car(string model, Engine engine, int year) 
        {
            this.model = model;
            this.motor = engine;
            this.year = year;
        }
    }

    public class Engine
    {
        [XmlElement(ElementName = "displacement")]
        private double displacement;
        [XmlElement(ElementName = "horsePower")]
        private double horsePower;
        [XmlAttribute(AttributeName = "model")]
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

        public Engine() { }

        public Engine(double displacement, double power, string model)
        {
            this.displacement = displacement;
            this.horsePower = power;
            this.model = model;
        }

    }

    public class Information
    {
        [XmlArray("car")]
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
                group f.hppl by f.engineType into g
                select new { name = g.Key, avg = g.Average()};                        
 
            //foreach (var value in result)
            //{
            //    Console.WriteLine(value.engineType);
            //}

            foreach (var value in result2)
            {
                Console.WriteLine("{0}: {1} ", value.name, value.avg);
            }

        }

        public void serialization()
        {
            XElement cars = new XElement("cars"); 
            
            foreach(var f in myCars) 
            {
                cars.Add(new XElement("car", 
                    new XElement("model", f.Model), 
                    new XElement("engine", 
                        new XElement("displacement", f.Engine.Displacement), 
                        new XElement("horsePower", f.Engine.HorsePower), 
                        new XAttribute("model", f.Engine.Model)), 
                    new XElement("year", f.Year))); 
            }

            cars.Save("CarsCollection.xml");

            XElement root = XElement.Load("CarsCollection.xml");
            //XDocument document = XDocument.Load("CarsCollection.xml");
            //XElement root2 = document.Root; 

            //Serializing Object
            Stream outputStream = File.OpenWrite("CarsCollection2.xml");
            Type type = typeof(List<Car>);
            XmlSerializer serializer = new XmlSerializer(type, new XmlRootAttribute("cars"));
            serializer.Serialize(outputStream, myCars);
            outputStream.Close();

            Stream inputStream = File.OpenRead("CarsCollection2.xml");
            XmlSerializer deserializer = new XmlSerializer(type, new XmlRootAttribute("cars"));
            var deser = (List<Car>)deserializer.Deserialize(inputStream);
            inputStream.Close();

        }

        public void createXmlFromLinq() {
            //LINQ query expressions
            IEnumerable<XElement> nodes = 
                                          from f in myCars
                                          select    new XElement("car",
                                                    new XElement("model", f.Model),
                                                    new XElement("engine",
                                                        new XElement("displacement", f.Engine.Displacement),
                                                        new XElement("horsePower", f.Engine.HorsePower),
                                                        new XAttribute("model", f.Engine.Model)),
                                                    new XElement("year", f.Year));
           
            XElement rootNode = new XElement("cars", nodes); //create a root node to contain the query results
            rootNode.Save("CarsFromLinq.xml");
        }

        public void XPath()
        {
            XElement rootNode = XElement.Load("CarsCollection.xml");
            //sumuję wszystkie wartości horsePower i dzielę przez ich ilość
            double avgHP = (double)rootNode.XPathEvaluate("(sum(/car/engine[@model!='TDI']/horsePower))div(count(/car/engine[@model!='TDI']/horsePower))");
            Console.WriteLine("XPath podpunkt 3:");
            Console.WriteLine("{0}", avgHP);
            
            IEnumerable<XElement> models = rootNode.XPathSelectElements("/car/model[not(. = preceding::car/model)]");
            foreach (var value in models)
            {
                Console.WriteLine("{0}", value);
            }
        }

        public void LINQtoXHTML()
        {
            IEnumerable<XElement> nodes =
                                          from f in myCars
                                          select new XElement("tr",
                                                    new XElement("td", f.Model),                                                    
                                                    new XElement("td", f.Engine.Displacement),
                                                    new XElement("td", f.Engine.HorsePower),
                                                    new XElement("td", f.Engine.Model),
                                                    new XElement("td", f.Year));

            XElement rootNode = new XElement("body",new XElement("table", nodes)); 

            XElement rootHTML = XElement.Load("template.html");
            rootHTML.LastNode.ReplaceWith(rootNode);
            //rootHTML.XPathSelectElement("//body").ReplaceNodes();
            rootHTML.Save("CarsToHtml.html");
        }

        public void modifyingXMLDocument()
        {
            XElement root = XElement.Load("CarsCollection.xml");
           
            //podpunkt 1 dla każdego węzła równolele przekształcam
            root.Elements("car").Elements("engine").Elements("horsePower").AsParallel()
                .ForAll(e => e.Name = "hp");

            //podpunkt 2
            //IEnumerable<XName> names = from a in root.Elements("car").Elements("year").Attributes() select a.Name;
            root.Elements("car").AsParallel()
               .ForAll(e => e.Element("model").SetAttributeValue("year", e.Element("year").Value));
            root.Elements("car").Elements("year").Remove();

            root.Save("carsmodyfikacionXMLDocument.xml");

        }

    }
}
