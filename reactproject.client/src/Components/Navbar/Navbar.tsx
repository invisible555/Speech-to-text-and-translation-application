import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../Context/AuthContext';
import './Navbar.css';

const Navbar: React.FC = () => {
  const { userLogin, logout } = useAuth();
  const [submenuOpen, setSubmenuOpen] = useState(false);
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 0);
    };
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  return (
    <nav
      aria-label="Główna nawigacja"
      className={`navbar fixed-top ${scrolled ? 'scrolled' : ''}`}
    >
      <ul className="menu">
        {/* Zawsze widoczne */}
        <li className="menu-item">
          <Link to="/" className="menu-link">Home</Link>
        </li>
        <li className="menu-item">
          <Link to="/about" className="menu-link">O nas</Link>
        </li>
        <li className="menu-item">
          <Link to="/contact" className="menu-link">Kontakt</Link>
        </li>

        {!userLogin && (
          <>
            <li className="menu-item">
              <Link to="/login" className="menu-link">Zaloguj się</Link>
            </li>
            <li className="menu-item">
              <Link to="/register" className="menu-link">Zarejestruj się</Link>
            </li>
          </>
        )}

        {userLogin && (
          <>
            <li className="menu-item">
              <Link to="/uploadfile" className="menu-link">Pliki</Link>
            </li>

            <li
              className="menu-item has-submenu"
              onMouseEnter={() => setSubmenuOpen(true)}
              onMouseLeave={() => setSubmenuOpen(false)}
            >
              <button
                aria-haspopup="menu"
                aria-expanded={submenuOpen}
                className="menu-button"
                onClick={() => setSubmenuOpen(!submenuOpen)}
              >
                {userLogin ?? 'Profil'}
              </button>

              {submenuOpen && (
                <ul className="submenu" role="menu">
                  <li role="menuitem">
                    <Link to="/profile">Zobacz profil</Link>
                  </li>
                  <li role="menuitem">
                    <Link to="/profile/edit">Edytuj profil</Link>
                  </li>
                  <li role="menuitem">
                    <button onClick={logout} className="submenu-button">Wyloguj się</button>
                  </li>
                </ul>
              )}
            </li>
          </>
        )}
      </ul>
    </nav>
  );
};

export default Navbar;
