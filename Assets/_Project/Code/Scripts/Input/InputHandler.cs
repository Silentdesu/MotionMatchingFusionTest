namespace BlackFire
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public sealed class InputHandler : MonoBehaviour
    {
        private Transform _transform = default;
        private Camera _camera = default;
        private Vector2 _input = default;

        private void Awake()
        {
            _transform = transform;
            _camera = Camera.main;
        }

        private void Update()
        {
            _input.x = Input.GetAxis("Horizontal");
            _input.y = Input.GetAxis("Vertical");
        }

        public NetworkInputData GetInput()
        {
            var data = new NetworkInputData();

            data.Direction = _camera.transform.forward * _input.y + _camera.transform.right * _input.x;
            data.Direction.y = 0.0f;
            // data.Direction = new Vector3(input.x, 0.0f, input.y);
            // data.Direction = _transform.forward * input.x + _transform.right * input.y;

            return data;
        }
    }
}
