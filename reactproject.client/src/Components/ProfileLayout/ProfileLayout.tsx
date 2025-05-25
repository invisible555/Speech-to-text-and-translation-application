import { Link, Outlet, useLocation } from "react-router-dom";

const ProfileLayout = () => {
  const location = useLocation();

  return (
    <div className="row">
      {/* Menu boczne po lewej */}
      <div className="col-md-3">
        <div className="list-group">
          <Link
            to="/profile"
            className={`list-group-item list-group-item-action ${location.pathname === "/profile" ? "active" : ""}`}
          >
            Podgląd profilu
          </Link>
          <Link
            to="/profile/edit"
            className={`list-group-item list-group-item-action ${location.pathname === "/profile/edit" ? "active" : ""}`}
          >
            Edytuj profil
          </Link>
          
        </div>
      </div>

      {/* Zawartość sekcji po prawej */}
      <div className="col-md-9">
        <Outlet />
      </div>
    </div>
  );
};

export default ProfileLayout;
