using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yipeee;

namespace Core
{
    public class ExtBodyPart : Feature
    {
        public float maxHealth;
        public float health;

        public ExtBodyPart(float maxHealth, string tableName) : base(tableName)
        {
            this.maxHealth = maxHealth;
            this.health = maxHealth;
            this.tableName = tableName;
        }

        public void Damage(float amount)
        {
            health = amount <= health ? health - amount : 0f;
        }
    }
}
