import { useEffect, useState } from 'react';

const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5075';
const emptyProject = { title: '', description: '', imageUrl: '', technologies: '', features: '', gitHubUrl: '', liveDemoUrl: '' };

export default function Admin() {
  const [login, setLogin] = useState({ email: '', password: '' });
  const [authenticated, setAuthenticated] = useState(false);
  const [project, setProject] = useState(emptyProject);
  const [projects, setProjects] = useState([]);
  const [messages, setMessages] = useState([]);
  const [status, setStatus] = useState('');

  const loadProjects = async () => {
    const response = await fetch(`${apiUrl}/api/projects`);
    if (response.ok) setProjects(await response.json());
  };

  const loadMessages = async () => {
    const response = await fetch(`${apiUrl}/api/contact-messages`, { credentials: 'include' });
    if (response.ok) setMessages(await response.json());
  };

  useEffect(() => {
    fetch(`${apiUrl}/api/auth/me`, { credentials: 'include' })
      .then((response) => {
        setAuthenticated(response.ok);
        if (response.ok) loadMessages();
      });
    loadProjects();
  }, []);

  const submitLogin = async (event) => {
    event.preventDefault();
    const response = await fetch(`${apiUrl}/api/auth/login`, {
      method: 'POST', credentials: 'include', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(login),
    });
    setAuthenticated(response.ok);
    if (response.ok) loadMessages();
    setStatus(response.ok ? 'Login successful.' : 'Invalid email or password.');
  };

  const addProject = async (event) => {
    event.preventDefault();
    const response = await fetch(`${apiUrl}/api/projects`, {
      method: 'POST', credentials: 'include', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(project),
    });
    if (response.ok) {
      setProject(emptyProject);
      setStatus('Project added successfully.');
      loadProjects();
    } else setStatus('Unable to add project. Please check all fields.');
  };

  const deleteProject = async (id) => {
    if (!window.confirm('Delete this project?')) return;
    const response = await fetch(`${apiUrl}/api/projects/${id}`, { method: 'DELETE', credentials: 'include' });
    if (response.ok) loadProjects();
  };

  const toggleMessage = async (item) => {
    const response = await fetch(`${apiUrl}/api/contact-messages/${item.id}/read?value=${!item.isRead}`, {
      method: 'PATCH', credentials: 'include',
    });
    if (response.ok) loadMessages();
  };

  const deleteMessage = async (id) => {
    if (!window.confirm('Delete this message?')) return;
    const response = await fetch(`${apiUrl}/api/contact-messages/${id}`, { method: 'DELETE', credentials: 'include' });
    if (response.ok) loadMessages();
  };

  if (!authenticated) return (
    <main className="admin-page"><form className="admin-card" onSubmit={submitLogin}>
      <a href="/">← Back to Portfolio</a><h1>Admin Login</h1><p>Manage your portfolio content.</p>
      <label>Email<input type="email" required value={login.email} onChange={(e) => setLogin({ ...login, email: e.target.value })} /></label>
      <label>Password<input type="password" required value={login.password} onChange={(e) => setLogin({ ...login, password: e.target.value })} /></label>
      {status && <p className="admin-status">{status}</p>}<button className="btn primary">Login</button>
    </form></main>
  );

  return <main className="admin-page"><div className="admin-shell">
    <header className="admin-header"><div><p className="eyebrow">PORTFOLIO ADMIN</p><h1>Projects</h1></div><a href="/">View Portfolio</a></header>
    <form className="admin-card admin-form" onSubmit={addProject}>
      <h2>Add New Project</h2>
      {Object.keys(emptyProject).map((name) => <label key={name}>{name.replace(/([A-Z])/g, ' $1')}
        {name === 'description' || name === 'features' ? <textarea required={name === 'description'} value={project[name]} onChange={(e) => setProject({ ...project, [name]: e.target.value })} /> : <input type={name.toLowerCase().includes('url') ? 'url' : 'text'} required={['title','technologies'].includes(name)} value={project[name]} onChange={(e) => setProject({ ...project, [name]: e.target.value })} />}
      </label>)}
      {status && <p className="admin-status">{status}</p>}<button className="btn primary">Add Project</button>
    </form>
    <section className="admin-list"><h2>Saved Projects</h2>{projects.length ? projects.map((item) => <article className="admin-project" key={item.id}><div><h3>{item.title}</h3><p>{item.technologies}</p></div><button onClick={() => deleteProject(item.id)}>Delete</button></article>) : <p>No projects added yet.</p>}</section>
    <section className="admin-list"><h2>Contact Messages</h2>{messages.length ? messages.map((item) => <article className={`admin-message ${item.isRead ? 'read' : ''}`} key={item.id}>
      <div className="message-meta"><span className="message-badge">{item.isRead ? 'Read' : 'New'}</span><time>{new Date(item.createdAt).toLocaleString()}</time></div>
      <h3>{item.subject}</h3><p><strong>{item.name}</strong> · <a href={`mailto:${item.email}`}>{item.email}</a>{item.phone && ` · ${item.phone}`}</p><p>{item.message}</p>
      <div className="message-actions"><button onClick={() => toggleMessage(item)}>Mark as {item.isRead ? 'Unread' : 'Read'}</button><button className="danger" onClick={() => deleteMessage(item.id)}>Delete</button></div>
    </article>) : <p>No contact messages yet.</p>}</section>
  </div></main>;
}
