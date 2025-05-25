import { useState } from 'react';
import ChangePasswordForm from '../ChangePassword/ChangePassword';

const EditLoginForm = () => (
  <form>
    <h4>Zmień login</h4>
    <input type="text" placeholder="Nowy login" className="form-control mb-2" />
    <button className="btn btn-primary">Zapisz</button>
  </form>
);

const EditEmailForm = () => (
  <form>
    <h4>Zmień email</h4>
    <input type="email" placeholder="Nowy email" className="form-control mb-2" />
    <button className="btn btn-primary">Zapisz</button>
  </form>
);

const ProfileEditor = () => {
  const [selectedSection, setSelectedSection] = useState<string>('login');

  const renderSection = () => {
    switch (selectedSection) {
      case 'login':
        return <EditLoginForm />;
      case 'email':
        return <EditEmailForm />;
      case 'password':
        return <ChangePasswordForm />;
      default:
        return null;
    }
  };

  return (
    <div className="row">
      <div className="col-md-3 border-end">
        <div className="list-group">
          <button onClick={() => setSelectedSection('login')} className="list-group-item list-group-item-action">
            Zmień login
          </button>
          <button onClick={() => setSelectedSection('email')} className="list-group-item list-group-item-action">
            Zmień email
          </button>
          <button onClick={() => setSelectedSection('password')} className="list-group-item list-group-item-action">
            Zmień hasło
          </button>
        </div>
      </div>
      <div className="col-md-9">
        {renderSection()}
      </div>
    </div>
  );
};

export default ProfileEditor;
