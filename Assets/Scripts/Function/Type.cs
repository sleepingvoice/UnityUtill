using System.Collections.Generic;
using UnityEngine.Events;

public class Type<T>
{
	T _value;   //값
	public T Value => _value;
	Queue<T> _queue = new Queue<T>(); //재귀방지용 큐
	UnityEvent<T> _update = new UnityEvent<T>();//실행될 함수
	bool _isRunning; //재귀방지용 값 저장 큐

	public void SetValue(T value)
	{
		_value = value;
		_queue.Enqueue(value);

		while (!_isRunning && _queue.TryDequeue(out T queueValue))
		{
			_isRunning = true;
			_update.Invoke(queueValue);
			_isRunning = false;
		};
	}


	public void AddListener(UnityAction<T> action)
	{
		_update.AddListener(action);
	}

	public void RemoveListenerAll()
	{
		_update.RemoveAllListeners();
	}

	public Type(T init = default)
	{
		_value = init;
	}
}
