using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MouseTester
{
    public class MouseEvent
    {
        public ushort buttonflags;
        public int lastx;
        public int lasty;
        public double ts;
        public int handle;

        public MouseEvent(int handle, ushort buttonflags, int lastx, int lasty, double ts)
        {
            this.handle = handle;
            this.buttonflags = buttonflags;
            this.lastx = lastx;
            this.lasty = lasty;
            this.ts = ts;
        }
    }
}
