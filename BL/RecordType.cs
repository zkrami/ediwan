using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Office.BL
{
    class RecordType
    {
        public int id;
        public string name;
        public RecordType(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
        public override string ToString()
        {
            return  " (" + id + ") "  + name; 
        }
        public override bool Equals(object obj)
        {
            if (obj is RecordType)
            {
                return this.id == (obj as RecordType).id; 
            }
            return base.Equals(obj);
        }
      
    }
}
