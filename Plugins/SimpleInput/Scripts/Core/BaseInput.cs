using UnityEngine;

namespace SimpleInputNamespace
{
	public interface IBaseInput
	{
		void StartTracking();
		void StopTracking();
		void ResetValue();
	}

	public abstract class BaseInput<K, V> : IBaseInput
	{
		[SerializeField]
		private K m_key;
		public K Key
		{
			get { return m_key; }
			set
			{
				if( !KeysEqual( m_key, value ) )
				{
					if( isTracking && IsKeyValid() )
						UnregisterInput();

					m_key = value;

					if( isTracking && IsKeyValid() )
						RegisterInput();
				}
			}
		}

		public V value;
		private bool isTracking = false;

		public BaseInput() { }

		public BaseInput( K key )
		{
			m_key = key;
		}

		public void StartTracking()
		{
			if( !isTracking )
			{
				if( IsKeyValid() )
					RegisterInput();

				isTracking = true;
			}
		}

		public void StopTracking()
		{
			if( isTracking )
			{
				if( IsKeyValid() )
					UnregisterInput();

				isTracking = false;
			}
		}

		public void ResetValue()
		{
			value = default( V );
		}

		protected abstract void RegisterInput();
		protected abstract void UnregisterInput();
		protected abstract bool KeysEqual( K key1, K key2 );
		
		public virtual bool IsKeyValid()
		{
			return true;
		}
	}
}