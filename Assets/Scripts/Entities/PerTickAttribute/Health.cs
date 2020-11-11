using Entities.Attributes;

namespace Entities.PerTickAttribute
{
    public class Health : PerTickAttribute
    {
        protected override void Start()
        {
            base.Start();
            OnEmpty += GetComponent<Entity>().Kill;
        }
    }
}
