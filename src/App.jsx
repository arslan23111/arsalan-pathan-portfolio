import { Mail, Menu, X } from 'lucide-react';
import { useEffect, useState } from 'react';
import profileImage from './assets/arsalan-developer.webp';
import { projects } from './data/projects';

const links = ['home', 'about', 'skills', 'services', 'experience', 'projects', 'certifications', 'resume', 'contact'];

const services = [
  {
    icon: '</>',
    title: 'Frontend Development',
    description: 'Responsive and user-friendly interfaces built with HTML, CSS, Bootstrap and JavaScript.',
  },
  {
    icon: '{ }',
    title: 'ASP.NET Web Development',
    description: 'Structured web applications developed using ASP.NET MVC and C#.',
  },
  {
    icon: 'DB',
    title: 'Database Integration',
    description: 'CRUD operations and reliable data management using SQL Server.',
  },
  {
    icon: 'API',
    title: 'API Integration',
    description: 'Connecting frontend interfaces with backend APIs for seamless data exchange.',
  },
];

export default function App() {
  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({ name: '', email: '', phone: '', subject: '', message: '' });
  const [formErrors, setFormErrors] = useState({});

  useEffect(() => {
    const elements = document.querySelectorAll(
      '.section, .skill, .service-card, .experience, .education, .projects-empty, .certification-card, .resume-card, .contact-form'
    );

    elements.forEach((element, index) => {
      element.classList.add('reveal');
      element.style.setProperty('--reveal-delay', `${(index % 4) * 90}ms`);
    });

    const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            entry.target.classList.add('is-visible');
            observer.unobserve(entry.target);
          }
        });
      },
      { threshold: 0.12, rootMargin: '0px 0px -50px' }
    );

    elements.forEach((element) => observer.observe(element));
    return () => observer.disconnect();
  }, []);

  const updateField = ({ target: { name, value } }) => {
    setFormData((current) => ({ ...current, [name]: value }));
    setFormErrors((current) => ({ ...current, [name]: '' }));
  };

  const submitContact = (event) => {
    event.preventDefault();
    const errors = {};
    if (formData.name.trim().length < 2) errors.name = 'Please enter your name.';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) errors.email = 'Please enter a valid email address.';
    if (formData.phone && !/^[+\d\s-]{7,18}$/.test(formData.phone)) errors.phone = 'Please enter a valid phone number.';
    if (formData.subject.trim().length < 3) errors.subject = 'Please enter a subject.';
    if (formData.message.trim().length < 10) errors.message = 'Message must contain at least 10 characters.';
    setFormErrors(errors);
    if (Object.keys(errors).length) return;

    const body = `Name: ${formData.name}\nEmail: ${formData.email}\nPhone: ${formData.phone || 'Not provided'}\n\n${formData.message}`;
    window.location.href = `mailto:lalaarslanpathan14@gmail.com?subject=${encodeURIComponent(formData.subject)}&body=${encodeURIComponent(body)}`;
  };

  return (
    <>
      <a className="skip-link" href="#main-content">Skip to main content</a>
      <div className="ambient-light ambient-one" aria-hidden="true" />
      <div className="ambient-light ambient-two" aria-hidden="true" />
      <header className="nav-wrap">
        <nav className="nav container" aria-label="Main navigation">
          <a className="logo" href="#home">AP<span>.</span></a>
          <button className="menu" onClick={() => setOpen(!open)} aria-label="Toggle navigation" aria-expanded={open} aria-controls="primary-navigation">{open ? <X /> : <Menu />}</button>
          <div id="primary-navigation" className={`nav-links ${open ? 'open' : ''}`}>
            {links.map(link => <a key={link} href={`#${link}`} onClick={() => setOpen(false)}>{link}</a>)}
          </div>
        </nav>
      </header>

      <main id="main-content">
        <section id="home" className="hero container">
          <div className="hero-copy">
            <p className="eyebrow">HELLO, I'M</p>
            <h1>Arsalan <span>Pathan</span></h1>
            <h2>Full-Stack Developer</h2>
            <p>I build responsive and user-friendly web applications using ASP.NET, C#, SQL Server and modern frontend technologies.</p>
            <div className="actions">
              <a className="btn primary" href="#experience">View Experience</a>
              <a className="btn secondary" href="#contact">Contact Me</a>
            </div>
            <div className="socials">
              <a href="https://github.com/arslan23111" target="_blank" rel="noreferrer" aria-label="GitHub">GitHub</a>
              <a href="https://www.linkedin.com/in/arsalan-pathan-55b78a299/" target="_blank" rel="noreferrer" aria-label="LinkedIn">LinkedIn</a>
              <a href="mailto:lalaarslanpathan14@gmail.com" aria-label="Email"><Mail /></a>
            </div>
          </div>
          <div className="profile-card">
            <img
              src={profileImage}
              alt="Arsalan Pathan, Full-Stack Developer"
              width="500"
              height="500"
              fetchPriority="high"
            />
          </div>
        </section>

        <section id="about" className="section alt"><div className="container narrow">
          <p className="eyebrow">ABOUT ME</p><h2>Building practical digital experiences</h2>
          <p>I am a Software Engineering student at the University of Sindh and a Full-Stack Developer with practical internship experience in responsive frontend development and ASP.NET applications. I enjoy creating clean interfaces, organizing content effectively and building reliable web features.</p>
        </div></section>

        <section id="skills" className="section container"><p className="eyebrow">MY TOOLKIT</p><h2>Skills & Technologies</h2>
          <div className="grid">{['HTML','CSS','Bootstrap','JavaScript','C#','ASP.NET MVC','SQL Server','Git & GitHub'].map(x => <div className="card skill" key={x}>{x}</div>)}</div>
        </section>

        <section id="services" className="section alt">
          <div className="container">
            <p className="eyebrow">WHAT I DO</p>
            <h2>Professional Services</h2>
            <div className="services-grid">
              {services.map((service) => (
                <article className="service-card" key={service.title}>
                  <span className="service-icon" aria-hidden="true">{service.icon}</span>
                  <h3>{service.title}</h3>
                  <p>{service.description}</p>
                </article>
              ))}
            </div>
          </div>
        </section>

        <section id="experience" className="section"><div className="container"><p className="eyebrow">EXPERIENCE</p><h2>Professional Journey</h2>
          <article className="experience"><div><strong>Jan 2025 — Mar 2026</strong><span>Hyderabad, Pakistan</span></div><div><h3>Frontend & ASP.NET Developer Intern</h3><h4>KX Soft Solution</h4><ul><li>Developed responsive interfaces using HTML, CSS, Bootstrap and JavaScript.</li><li>Worked with ASP.NET MVC, C#, APIs and CRUD operations.</li><li>Improved page layouts, content organization and visual consistency.</li><li>Used SQL Server for database operations and data management.</li></ul></div></article>
          <article className="education"><p className="eyebrow">EDUCATION</p><h3>BS Software Engineering</h3><p>University of Sindh, Hyderabad · 2023–2027 · Currently Studying</p></article>
        </div></section>

        <section id="projects" className="section container">
          <div className="center">
            <p className="eyebrow">SELECTED WORK</p>
            <h2>Featured Projects</h2>
          </div>
          {projects.length ? (
            <div className="projects-grid">
              {projects.map((project) => (
                <article className="project-card" key={project.title}>
                  <img src={project.image} alt={`${project.title} project preview`} loading="lazy" />
                  <div className="project-content">
                    <h3>{project.title}</h3>
                    <p>{project.description}</p>
                    <div className="tags">{project.technologies.map((technology) => <span key={technology}>{technology}</span>)}</div>
                    <ul>{project.features.map((feature) => <li key={feature}>{feature}</li>)}</ul>
                    <div className="project-links">
                      {project.github && <a href={project.github} target="_blank" rel="noreferrer">GitHub Repository</a>}
                      {project.demo && <a href={project.demo} target="_blank" rel="noreferrer">Live Demo</a>}
                    </div>
                  </div>
                </article>
              ))}
            </div>
          ) : (
            <div className="projects-empty center">
              <span aria-hidden="true">&lt;/&gt;</span>
              <h3>Projects Coming Soon</h3>
              <p>Selected development projects and case studies will be added shortly.</p>
            </div>
          )}
        </section>

        <section id="certifications" className="section alt">
          <div className="container certification-card">
            <div className="certificate-mark" aria-hidden="true">✓</div>
            <div>
              <p className="eyebrow">CERTIFICATIONS & ACHIEVEMENTS</p>
              <h2>Continuous Learning</h2>
              <p>Professional certifications, training programs and achievements will be added soon.</p>
            </div>
          </div>
        </section>

        <section id="resume" className="section">
          <div className="container resume-card">
            <div>
              <p className="eyebrow">PROFESSIONAL PROFILE</p>
              <h2>View My Resume</h2>
              <p>My updated professional resume will be available here shortly.</p>
            </div>
            <button className="btn resume-disabled" type="button" disabled>
              Resume Coming Soon
            </button>
          </div>
        </section>

        <section id="contact" className="section contact">
          <div className="container contact-layout">
            <div className="contact-copy">
              <p className="eyebrow">GET IN TOUCH</p>
              <h2>Let's work together</h2>
              <p>Have an opportunity or project in mind? Send me a message and I will get back to you.</p>
              <a href="mailto:lalaarslanpathan14@gmail.com">lalaarslanpathan14@gmail.com</a>
              <p>+92 335 5662869<br />Hyderabad, Sindh, Pakistan</p>
            </div>
            <form className="contact-form" onSubmit={submitContact} noValidate>
              <div className="form-row">
                <label>Name<input name="name" value={formData.name} onChange={updateField} placeholder="Your name" />{formErrors.name && <small>{formErrors.name}</small>}</label>
                <label>Email<input type="email" name="email" value={formData.email} onChange={updateField} placeholder="you@example.com" />{formErrors.email && <small>{formErrors.email}</small>}</label>
              </div>
              <div className="form-row">
                <label>Phone <span>(optional)</span><input type="tel" name="phone" value={formData.phone} onChange={updateField} placeholder="+92 300 0000000" />{formErrors.phone && <small>{formErrors.phone}</small>}</label>
                <label>Subject<input name="subject" value={formData.subject} onChange={updateField} placeholder="Project inquiry" />{formErrors.subject && <small>{formErrors.subject}</small>}</label>
              </div>
              <label>Message<textarea name="message" rows="5" value={formData.message} onChange={updateField} placeholder="Tell me about your project..." />{formErrors.message && <small>{formErrors.message}</small>}</label>
              <button className="btn primary" type="submit">Send Message</button>
            </form>
          </div>
        </section>
      </main>
      <footer>
        <div className="container footer-content">
          <div>
            <a className="logo" href="#home">AP<span>.</span></a>
            <p>Full-Stack Developer creating responsive and reliable web experiences.</p>
          </div>
          <div className="footer-links" aria-label="Social profiles">
            <a href="https://github.com/arslan23111" target="_blank" rel="noreferrer">GitHub</a>
            <a href="https://www.linkedin.com/in/arsalan-pathan-55b78a299/" target="_blank" rel="noreferrer">LinkedIn</a>
            <a href="mailto:lalaarslanpathan14@gmail.com">Email</a>
          </div>
          <p>© 2026 Arsalan Pathan. Built with React.</p>
        </div>
      </footer>
    </>
  );
}
