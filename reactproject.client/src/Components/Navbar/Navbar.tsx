import React, { useState , useEffect} from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../Context/AuthContext';
import './Navbar.css'; // Importing CSS file
import AudioUpload from '../AudioUpload/AudioUpload';

const Navbar: React.FC = () => {
  const { userLogin, logout } = useAuth();
  const [isMenuOpen, setIsMenuOpen] = useState(false); // Stan dla menu
  const [scrolled, setScrolled] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 0);
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);
  const handleToggleMenu = () => {
    setIsMenuOpen(!isMenuOpen); // Przełączamy stan rozwinięcia menu
  };

  return (
     <nav className={`navbar navbar-expand-lg navbar-dark fixed-top ${scrolled ? 'scrolled' : ''}`} style={{ backgroundColor: '#333' }}>
      <div className="container-fluid">
        <Link to="/" className="navbar-brand">Logo</Link>
        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#navbarNav"
          aria-controls="navbarNav"
          aria-expanded={isMenuOpen ? 'true' : 'false'}
          aria-label="Toggle navigation"
          onClick={handleToggleMenu} // Dodajemy obsługę kliknięcia
        >
          <span className={`navbar-toggler-icon ${isMenuOpen ? 'open' : ''}`}></span>
        </button>
        <div className={`collapse navbar-collapse ${isMenuOpen ? 'show' : ''}`} id="navbarNav">
          <ul className="navbar-nav">
            <li className="nav-item">
              <Link to="/about" className="nav-link">O nas</Link>
            </li>
            <li className="nav-item">
              <Link to="/contact" className="nav-link">Kontakt</Link>
            </li>

            {userLogin ? (
              <>
                <li className="nav-item">
                  <span className="nav-link">Witaj, {userLogin}</span>
                </li>
                <li className="nav-item">
                  <button className="btn btn-link nav-link" onClick={logout}>Wyloguj się</button>
                </li>
                <li className="nav-item">
                  <Link to="/uploadfile" className="nav-link">Pliki</Link>
                </li>
              </>
            ) : (
              <>
                <li className="nav-item">
                  <Link to="/login" className="nav-link">Zaloguj się</Link>
                </li>
                <li className="nav-item">
                  <Link to="/register" className="nav-link">Zarejestruj się</Link>
                </li>
              </>
            )}
          </ul>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
