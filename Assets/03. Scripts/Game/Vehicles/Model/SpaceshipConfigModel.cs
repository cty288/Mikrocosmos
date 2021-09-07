using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MikroFramework.DataStructures;
using MikroFramework.Singletons;

public class SpaceshipConfigModel:Table<SpaceshipConfig>, ISingleton {
    public TableIndex<string, SpaceshipConfig> NameIndex { get; private set; }
    public TableIndex<int, SpaceshipConfig> IdIndex { get; private set; }


    public static SpaceshipConfigModel Singleton {
        get {
            return SingletonProperty<SpaceshipConfigModel>.Singleton;
        }
    }

    private SpaceshipConfigModel() {
        
    }

    protected override void OnClear() {
        NameIndex.Clear();
        IdIndex.Clear();
    }

    public override void OnAdd(SpaceshipConfig item) {
       NameIndex.Add(item);
       IdIndex.Add(item);
    }

    public override void OnRemove(SpaceshipConfig item) {
        NameIndex.Remove(item);
        IdIndex.Remove(item);
    }

    public void OnSingletonInit() {
        NameIndex = new TableIndex<string, SpaceshipConfig>(item => item.Name);
        IdIndex = new TableIndex<int, SpaceshipConfig>(item => item.Id);
    }
}

