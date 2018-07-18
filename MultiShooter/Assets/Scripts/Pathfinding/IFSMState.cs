
public interface IFSMState<T> {
	void Enter(T entity);
	void Reason(T entity);
	void Update(T entity);
	void Exit(T entity);
}

// Another possibility would be to use an abstract class. Like so:
//public abstract class FSMState<T>
//{
//  abstract public void Enter(T entity);
//    abstract public void Reason(T entity);
//  abstract public void Update(T entity);
//  abstract public void Exit(T entity);
//}
//
// This would be suitable if there was any internal state in the form of variables, which all FSMStates 
// would share. We could, e.g., store an owner instead of passing an entity into every method. 
