using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdr.JsonMagic;

namespace SimpleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //            JsConfig.With(maxDepth: 1);

                EntityModel model = new EntityModel("TestModel");

                IGxPoint pt1 = new GxPoint();
                pt1.Name = "pt1";
                pt1.X = 100;

                IGxPoint pt2 = new GxPoint();
                pt2.Name = "pt2";
                pt2.X = 200;
                pt2.AddPar(pt1);

                model.AddEntity(pt1);
                model.AddEntity(pt2);

                string j;

                j = JsonMagic.ToJson(new List<int> { 1, 2, 3, 4 });

                j = JsonMagic.ToJson(model);

                j = JsonMagic.ToJson("Hello");

                j = JsonMagic.ToJson(new[] { 10, 20, 30 });

/*
                var json = JsonConvert.SerializeObject(model, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new[] { new MyConverter(), }
                });

                j = JsonConvert.SerializeObject("Hello");
                j = JsonMagic.ToJson("Hello");

                j = JsonConvert.SerializeObject(new[] { 10, 20, 30 });
                j = JsonMagic.ToJson(new[] { 10, 20, 30 });

                var mdl = JsonConvert.DeserializeObject<EntityModel>(json, new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                    TypeNameHandling = TypeNameHandling.Auto,
                    Converters = new[] { new MyConverter(), }
                });
*/

                //            var json = JsonSerializer.SerializeToString(model);

                //            var mdl = JsonSerializer.DeserializeFromString<EntityModel>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: {0}", ex);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

/*
    class MyConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IDictionary<IGxEntity, int>)
            {
                var list = ((IDictionary<IGxEntity, int>)value).ToList();

                serializer.Serialize(writer, list);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            JObject jobj = JObject.Load(reader);

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            Console.WriteLine("CanConvert: {0}", objectType);

            return typeof(IDictionary<IGxEntity, int>).IsAssignableFrom(objectType);
        }
    }
*/

    /// <summary>
    /// 
    /// </summary>
    public class EntityModel
    {

        public string Name { get; set; }
        public IList<IGxEntity> Entities { get; set; }

        public EntityModel(string name)
        {
            Name = name;
        }

        public void AddEntity(IGxEntity entity)
        {
            if (Entities == null)
            {
                Entities = new List<IGxEntity>();
            }

            Entities.Add(entity);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IGxEntity
    {
        string Name { get; set; }
        IDictionary<IGxEntity, int> Dependants { get; set; }
        IDictionary<IGxEntity, int> Parents { get; set; }

        void AddDep(IGxEntity dep);
        void AddPar(IGxEntity dep);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IGxPoint : IGxEntity
    {
        int X { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class GxEntityBase : IGxEntity
    {
        private IDictionary<IGxEntity, int> _dependants;
        private IDictionary<IGxEntity, int> _parents;

        public GxEntityBase()
        {
            Dependants = new Dictionary<IGxEntity, int>();
            Parents = new Dictionary<IGxEntity, int>();
        }

        public string Name { get; set; }

        public IDictionary<IGxEntity, int> Dependants
        {
            get { return _dependants; }
            set
            {
                _dependants = value;
            }
        }

        public IDictionary<IGxEntity, int> Parents
        {
            get { return _parents; }
            set { _parents = value; }
        }

        public void AddDep(IGxEntity dep)
        {
            if (_dependants.ContainsKey(dep))
                _dependants[dep]++;
            else
            {
                _dependants.Add(dep, 1);

                dep.AddPar(this);
            }
        }

        public void AddPar(IGxEntity par)
        {
            if (_parents.ContainsKey(par))
                _parents[par]++;
            else
            {
                _parents.Add(par, 1);

                par.AddDep(this);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GxPoint : GxEntityBase, IGxPoint
    {
        public int X { get; set; }
    }

}
