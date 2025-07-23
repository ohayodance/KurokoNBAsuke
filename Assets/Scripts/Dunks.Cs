using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Animator))] // Требуем наличие Animator
    public class Dunks : MonoBehaviour
    {
        [Header("Dunk Settings")]
        public float dunkPower = 1.0f;
        public float dunkHeight = 1.5f;
        public float dunkDistanceThreshold = 5f;

        private Animator animator;
        private bool isDunking = false;

        void Start()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Dunks component requires Animator!");
            }
        }

        public void HandleDunks(bool hasBall, bool jumpInput, float moveInput, Vector3 targetPosition)
        {
            if (hasBall && !isDunking && jumpInput)
            {
                float distanceToHoop = Vector3.Distance(transform.position, targetPosition);

                if (distanceToHoop < dunkDistanceThreshold)
                {
                    DunkType dunkType = GetRandomDunkType();
                    PerformDunk(dunkType, moveInput);
                }
            }
        }

        private DunkType GetRandomDunkType()
        {
            int randomValue = Random.Range(0, 5);
            return (DunkType)randomValue;
        }

        private void PerformDunk(DunkType dunkType, float moveInput)
        {
            isDunking = true;
            dunkPower = 1.0f + (moveInput * 0.5f);

            switch (dunkType)
            {
                case DunkType.TwoHanded:
                    animator.SetTrigger("TwoHandDunk");
                    break;
                case DunkType.OneHanded:
                    animator.SetTrigger("OneHandDunk");
                    break;
                case DunkType.Windmill:
                    animator.SetTrigger("WindmillDunk");
                    break;
                case DunkType.ThreeSixty:
                    animator.SetTrigger("360Dunk");
                    break;
                case DunkType.BetweenLegs:
                    animator.SetTrigger("BetweenLegsDunk");
                    break;
            }
        }

        // Вызывается через Animation Event в конце анимации данка
        public void OnDunkComplete()
        {
            isDunking = false;
            // Здесь можно добавить дополнительные эффекты
        }

        private enum DunkType
        {
            TwoHanded,
            OneHanded,
            Windmill,
            ThreeSixty,
            BetweenLegs
        }
    }
}
