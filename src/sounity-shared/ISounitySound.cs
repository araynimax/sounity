using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sounity
{
    public interface ISounitySound
    {
        void Start();

        void Move(float posX, float posY, float posZ);

        void Rotate(float rotX, float rotY, float rotZ);

        void Stop();

        void Dispose();

        void Attach(int entityId);

        void Detach();
    }
}
