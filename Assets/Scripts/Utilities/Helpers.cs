using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using UnityEngine;
using System.Collections;

/// <summary>
/// A static class for general helpful methods
/// </summary>
public static class Helpers 
{
    /// <summary>
    /// Destroy all child objects of this transform (Unintentionally evil sounding).
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren();
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform t) {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }

    public static TweenerCore<Vector3, Vector3, VectorOptions> DOMoveInTargetLocalSpace(this Transform transform, Transform target, Vector3 targetLocalEndPosition, float duration)
    {
        var t = DOTween.To(
            () => transform.position - target.transform.position, // Value getter
            x => transform.position = x + target.transform.position, // Value setter
            targetLocalEndPosition,
            duration);
        t.SetTarget(transform);
        return t;
    }

    public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }

    public static IEnumerator MoveUntilDie(Transform myTransform, GameObject target, Sequence tweener)
    {
        tweener.onKill += () => tweener = null;

        while (tweener != null && tweener.IsPlaying())
        {
            tweener.Restart();
            yield return new WaitForEndOfFrame();
        }

        yield return null;
    }

    /**
     * Cast a ray from the camera to the back of the grabbed object.
     */
    public static bool HitBehindGrabbedObject(GameObject grabbedObject, out RaycastHit hit, float maxDistance = Mathf.Infinity)
    {

        if (!grabbedObject) return Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit); ;

        int oldLayer = grabbedObject.layer;
        grabbedObject.layer = 2;
        bool b = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, ~(1 << 2));
        grabbedObject.layer = oldLayer;

        return b;
    }

    public static bool HitBehindGrabbedObjectFromHand(GameObject hand, GameObject grabbedObject, out RaycastHit hit, float maxDistance = Mathf.Infinity)
    {
        if (!grabbedObject) return Physics.Raycast(hand.transform.position, hand.transform.forward, out hit);

        int oldLayer = grabbedObject.layer;
        grabbedObject.layer = 2;
        bool b = Physics.Raycast(hand.transform.position, hand.transform.forward, out hit, maxDistance, ~(1 << 2));
        grabbedObject.layer = oldLayer;
        return b;
    }
}
