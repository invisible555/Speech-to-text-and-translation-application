const ChangePasswordForm = () => {
  return (
    <form>
      <h4>Zmień hasło</h4>
      <input type="password" placeholder="Stare hasło" className="form-control mb-2" />
      <input type="password" placeholder="Nowe hasło" className="form-control mb-2" />
      <input type="password" placeholder="Powtórz nowe hasło" className="form-control mb-2" />
      <button className="btn btn-primary">Zapisz</button>
    </form>
  );
};

export default ChangePasswordForm;
