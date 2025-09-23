using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spaceInvaders2
{
    internal class CooldownTimer
{
    private double currentTime = 0.0;

    public void ResetAndStart(double delay) {currentTime = delay; }

    public bool IsDone() {return currentTime <= 0.0; }

    public void Update(double deltaTime) { currentTime -= deltaTime; }
}
}
