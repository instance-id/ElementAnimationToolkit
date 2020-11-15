// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/AboveAverageInspector         --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEngine;

namespace instance.id.EATK.Examples
{
    [CreateAssetMenu(fileName = "Axe.asset", menuName = "instance.id/Example/Axe", order = 0)]
    public class Axe
        : WeaponBase
    {
        public int Damage;
        public int Weight;
        public int Durability;
        public Texture2D icon;
    }
}
