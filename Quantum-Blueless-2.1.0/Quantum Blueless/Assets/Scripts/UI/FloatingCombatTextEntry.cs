using TMPro;
using UnityEngine;

public class FloatingCombatTextEntry : MonoBehaviour
{
	[Header( "References" )]
	public TMP_Text textElement;
	public RectTransform rectTransform;

	[Header( "Configurations" )]
	public AnimationCurve alphaCurve;
	public AnimationCurve xOffsetCurve;
	public AnimationCurve yOffsetCurve;
	public AnimationCurve scaleCurve;

	public float animationTime;
	public Vector2 offset;
	public Vector2 offsetVariance;
	public Vector2 offsetVarianceScale = new Vector2( 10.0f, 10.0f );

	public System.Action<FloatingCombatTextEntry> onAnimationFinished;

	private float timer = -1;
	private Vector2 targetOffset;

	public void Activate( int damage, Vector2 directionHint )
	{
		textElement.text = damage.ToString();
		textElement.enabled = true;
		enabled = true;

		textElement.rectTransform.anchoredPosition = Vector3.zero;
		textElement.alpha = 1;
		timer = 0;

		if( directionHint.x > 0.0f )
			targetOffset.x = Random.Range( 0.0f, offsetVariance.x ) * offsetVarianceScale.x + offset.x;
		else
			targetOffset.x = Random.Range( -offsetVariance.x, 0.0f ) * offsetVarianceScale.x - offset.x;

		targetOffset.y = Random.Range( -offsetVariance.y, offsetVariance.y ) * offsetVarianceScale.y + offset.y;
	}

	public void Deactivate()
	{
		timer = -1;
		textElement.enabled = false;
		enabled = false;

		if( onAnimationFinished != null )
			onAnimationFinished( this );
	}

	private void Update()
	{
		timer += Time.deltaTime;
		float t = Mathf.Clamp01( timer / animationTime );

		textElement.alpha = alphaCurve.Evaluate( t );

		var position = targetOffset;
		position.x *= xOffsetCurve.Evaluate( t );
		position.y *= yOffsetCurve.Evaluate( t );
		textElement.rectTransform.anchoredPosition = position;

		var scale = scaleCurve.Evaluate( t );
		textElement.rectTransform.localScale = new Vector3( scale, scale, scale );

		if( timer > animationTime )
			Deactivate();
	}
}