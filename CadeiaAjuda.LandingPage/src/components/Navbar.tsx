import { useState } from 'react'
import { useNavbarScroll } from '../hooks/useNavbarScroll'

export default function Navbar() {
  const scrolled = useNavbarScroll()
  const [menuOpen, setMenuOpen] = useState(false)
  const appUrl = import.meta.env.DEV
    ? 'https://app.dev.assistchain.com.br'
    : 'https://app.assistchain.com.br'

  const handleLinkClick = () => {
    setMenuOpen(false)
  }

  return (
    <nav className={`navbar${scrolled ? ' scrolled' : ''}`} id="navbar">
      <div className="container nav-container">
        <a href="#" className="nav-logo" aria-label="AssistChain">
          <img src="/image/logo/AssistChain-logo.png" alt="AssistChain" className="logo-img" />
        </a>
        <div className={`nav-links${menuOpen ? ' open' : ''}`} id="navLinks">
          <a href="#funcionalidades" onClick={handleLinkClick}>Funcionalidades</a>
          <a href="#como-funciona" onClick={handleLinkClick}>Como Funciona</a>
          <a href="#tecnologias" onClick={handleLinkClick}>Tecnologias</a>
          <a href="#contato" onClick={handleLinkClick}>Contato</a>
        </div>
        <div className="nav-actions">
          <a href={appUrl} className="nav-btn-enter">
            <i className="fas fa-sign-in-alt"></i> Entrar
          </a>
          <button
            className="mobile-menu-btn"
            id="mobileMenuBtn"
            aria-label="Menu"
            onClick={() => setMenuOpen(!menuOpen)}
          >
            <i className={`fas ${menuOpen ? 'fa-times' : 'fa-bars'}`}></i>
          </button>
        </div>
      </div>
    </nav>
  )
}
