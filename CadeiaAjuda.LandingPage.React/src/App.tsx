import Navbar from './components/Navbar'
import Hero from './components/Hero'
import Stats from './components/Stats'
import PlatformDemo from './components/PlatformDemo'
import Features from './components/Features'
import AndonShowcase from './components/AndonShowcase'
import HowItWorks from './components/HowItWorks'
import TechStack from './components/TechStack'
import CTA from './components/CTA'
import Footer from './components/Footer'
import ScrollProgress from './components/ScrollProgress'
import { useScrollAnimations } from './hooks/useScrollAnimations'

function App() {
  useScrollAnimations()

  return (
    <>
      <ScrollProgress />
      <Navbar />
      <Hero />
      <Stats />
      <PlatformDemo />
      <Features />
      <AndonShowcase />
      <HowItWorks />
      <TechStack />
      <CTA />
      <Footer />
    </>
  )
}

export default App
