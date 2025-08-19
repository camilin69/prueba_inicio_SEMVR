using UnityEngine;
using System.Collections.Generic;

public class HandFistController : MonoBehaviour
{
    [Header("Teclas")]
    public KeyCode fistKey = KeyCode.T;     // Cerrar mano
    public KeyCode openKey = KeyCode.R;     // Abrir mano

    [Header("Huesos del Pulgar")]
    public Transform thumbMetacarpal;
    public Transform thumbProximal;
    public Transform thumbDistal;
    public Transform thumbTip;

    [Header("Huesos de los Demás Dedos")]
    public List<Transform> indexBones = new List<Transform>();
    public List<Transform> middleBones = new List<Transform>();
    public List<Transform> ringBones = new List<Transform>();
    public List<Transform> pinkyBones = new List<Transform>();

    [Header("Ajustes de Animación")]
    [Range(0.1f, 2f)] public float animationSpeed = 0.5f;

    [Header("Timing")]
    [Range(1f, 2f)] public float animationDelay = 1.5f;

    [Header("Semáforo de Estado")]
    public Renderer stateCube; 
    public Color idleColor = Color.red;
    public Color waitingColor = Color.yellow;
    public Color animatingColor = Color.green;

    private int handDirection = 1; // 1 para izquierda, -1 para derecha

    private bool isAnimationLocked = false;
    
    // Ángulos para el puño (otros dedos)
    private readonly float[] fistAngles = { 50f, 70f, 60f, 40f };

   
    
    // Almacenamiento de rotaciones originales
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();
    private Dictionary<Transform, Vector3> originalPositions = new Dictionary<Transform, Vector3>(); 
    private float blendAmount;

    

    void Start()
    {
        
        string handName = gameObject.name;
        handDirection = handName.Contains("Right") ? -1 : 1;

        // Registrar todas las rotaciones originales
        RegisterBones(thumbMetacarpal, thumbProximal, thumbDistal, thumbTip);
        RegisterOriginalPositions();
        RegisterFinger(indexBones);
        RegisterFinger(middleBones);
        RegisterFinger(ringBones);
        RegisterFinger(pinkyBones);

        SetStateColor(idleColor);
    }

    void Update()
    {
        if (isAnimationLocked) return;

        if (Input.GetKeyDown(fistKey))
        {
            StartCoroutine(AnimateHand(true));
        }
        else if (Input.GetKeyDown(openKey))
        {
            StartCoroutine(AnimateHand(false));
        }
        
    }

    // Corrutina renombrada para evitar conflictos
    private System.Collections.IEnumerator AnimateHand(bool makeFist)
    {
        isAnimationLocked = true;

        SetStateColor(waitingColor);
        
        yield return new WaitForSeconds(animationDelay);
        
        SetStateColor(animatingColor);
        
        float targetBlend = makeFist ? 1f : 0f;
        float startBlend = blendAmount;
        float duration = 1.0f / animationSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            blendAmount = Mathf.Lerp(startBlend, targetBlend, elapsed / duration);
            ApplyAllPosesImmediate();
            elapsed += Time.deltaTime;
            yield return null;
        }

        blendAmount = targetBlend;
        ApplyAllPosesImmediate();
        
        SetStateColor(idleColor);
        
        isAnimationLocked = false;
    }

    private void SetStateColor(Color color)
    {
        if (stateCube != null)
        {
            stateCube.material.color = color;
        }
    }

    private void ApplyAllPosesImmediate()
    {
        ApplyThumbPoseImmediate();
        ApplyFingerPoseImmediate(indexBones, fistAngles);
        ApplyFingerPoseImmediate(middleBones, fistAngles);
        ApplyFingerPoseImmediate(ringBones, fistAngles);
        ApplyFingerPoseImmediate(pinkyBones, fistAngles, true);
    }

    private void ApplyThumbPoseImmediate()
    {
        float yRotation = handDirection == 1 ? 5f : -5f;
        
        thumbMetacarpal.localRotation = originalRotations[thumbMetacarpal] * 
            Quaternion.Euler(-15f * blendAmount, yRotation * blendAmount, 0);
            
        thumbProximal.localRotation = originalRotations[thumbProximal] * 
            Quaternion.Euler(-26f * blendAmount, 0, 0);
            
        thumbDistal.localRotation = originalRotations[thumbDistal] * 
            Quaternion.Euler(-40f * blendAmount, 0, 0);
            
        thumbTip.localRotation = originalRotations[thumbTip] * 
            Quaternion.Euler(0, 0, 0);
    }

    private void ApplyFingerPoseImmediate(List<Transform> bones, float[] angles, bool isPinky = false)
    {
        if (bones == null || bones.Count == 0) return;
        
        for (int i = 0; i < bones.Count; i++)
        {
            int angleIndex = Mathf.Min(i, angles.Length - 1);
            float zRotation = 0f;

            if (isPinky) 
            {
                zRotation = Mathf.Lerp(0, -25f, i / (float)(bones.Count - 1));
            }
            
            Quaternion rotation = Quaternion.Euler(
                angles[angleIndex] * blendAmount, 
                0, 
                blendAmount * (-handDirection * zRotation)
            );
            
            bones[i].localRotation = originalRotations[bones[i]] * rotation;

            // Aplicar traslación si es necesario
            if (isPinky && originalPositions.ContainsKey(bones[i]))
            {
                Vector3 newPosition = originalPositions[bones[i]];
                newPosition.x += (-handDirection * 0.0056f) * blendAmount; 
                bones[i].localPosition = newPosition;
            }
        }
    }

    private void RegisterBones(params Transform[] bones)
    {
        foreach (var bone in bones)
        {
            if (bone && !originalRotations.ContainsKey(bone))
                originalRotations.Add(bone, bone.localRotation);
        }
    }

    private void RegisterOriginalPositions()
    {
        RegisterBonePosition(thumbMetacarpal, thumbProximal, thumbDistal, thumbTip);
        RegisterFingerPositions(indexBones);
        RegisterFingerPositions(middleBones);
        RegisterFingerPositions(ringBones);
        RegisterFingerPositions(pinkyBones); 
    }

    private void RegisterBonePosition(params Transform[] bones)
    {
        foreach (var bone in bones)
        {
            if (bone && !originalPositions.ContainsKey(bone))
                originalPositions.Add(bone, bone.localPosition);
        }
    }

    private void RegisterFingerPositions(List<Transform> bones)
    {
        foreach (var bone in bones)
        {
            if (bone && !originalPositions.ContainsKey(bone))
                originalPositions.Add(bone, bone.localPosition);
        }
    }

    private void RegisterFinger(List<Transform> bones)
    {
        foreach (var bone in bones)
        {
            if (bone && !originalRotations.ContainsKey(bone))
                originalRotations.Add(bone, bone.localRotation);
        }
    }

    
    // Método para depuración
    void OnDrawGizmosSelected()
    {
        if (thumbMetacarpal != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(thumbMetacarpal.position, 0.005f);
        }
    }
}
