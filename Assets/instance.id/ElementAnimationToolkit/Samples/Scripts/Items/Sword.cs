// ----------------------------------------------------------------------------
// -- Project : https://github.com/instance-id/ElementAnimationToolkit       --
// -- instance.id 2020 | http://github.com/instance-id | http://instance.id  --
// ----------------------------------------------------------------------------

using UnityEngine;

namespace instance.id.EATK.Examples
{
    [CreateAssetMenu(fileName = "Sword.asset", menuName = "instance.id/Example/Sword", order = 0)]
    public class Sword : WeaponBase
    {
        public int Damage;
        public int Weight;
        public int Durability;
        public Texture2D icon;
    }
}
