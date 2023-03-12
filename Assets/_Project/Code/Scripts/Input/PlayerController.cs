namespace BlackFire
{
    using Cinemachine;
    using Fusion;
    using MxM;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkCharacterControllerPrototype))]
    public class PlayerController : NetworkBehaviour
    {
        private NetworkCharacterControllerPrototype _controller = default;
        private MxMTrajectoryGenerator _trajectoryGenerator = default;

        [field: SerializeField]
        public static PlayerController Local { get; set; }

        private void Awake()
        {
            _controller = GetComponent<NetworkCharacterControllerPrototype>();
            _trajectoryGenerator = GetComponent<MxMTrajectoryGenerator>();
        }

        public override void Spawned()
        {
            if (Object.HasInputAuthority)
            {
                Local = this;
                Debug.Log($"Local Player");

                var cinemachine = FindObjectOfType<CinemachineFreeLook>();

                if (cinemachine != null)
                {
                    cinemachine.LookAt = transform;
                    cinemachine.Follow = transform;
                }
            }

            Debug.Log("Non local player");
        }

        public override void FixedUpdateNetwork()
        {
            Debug.Log($"FixedUpdateNetwork tick!");

            if (GetInput(out NetworkInputData data))
            {
                Debug.Log($"Data: {data.Direction}");

                data.Direction.Normalize();
                OnTrajectoryRPC(data);
                _controller.Move(data.Direction * Runner.DeltaTime);
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        private void OnTrajectoryRPC(NetworkInputData data)
        {
            _trajectoryGenerator?.SetInput(new Vector2(data.Direction.x, data.Direction.z));
        }
    }
}