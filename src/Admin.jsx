import { useEffect, useState } from 'react';

const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5075';
const tokenKey = 'portfolio_admin_token';
const authHeaders = () => {
  const token = sessionStorage.getItem(tokenKey);
  return token ? { Authorization: `Bearer ${token}` } : {};
};
const emptyProject = { title: '', description: '', imageUrl: '', technologies: '', features: '', gitHubUrl: '', liveDemoUrl: '' };
const emptyCertificate = { title: '', issuer: '', issueYear: new Date().getFullYear(), description: '', fileUrl: '', fileType: '' };

export default function Admin() {
  const [login, setLogin] = useState({ email: '', password: '' });
  const [authenticated, setAuthenticated] = useState(false);
  const [project, setProject] = useState(emptyProject);
  const [projects, setProjects] = useState([]);
  const [messages, setMessages] = useState([]);
  const [certificates, setCertificates] = useState([]);
  const [certificate, setCertificate] = useState(emptyCertificate);
  const [certificateFile, setCertificateFile] = useState(null);
  const [editingCertificateId, setEditingCertificateId] = useState(null);
  const [status, setStatus] = useState('');
  const [editingId, setEditingId] = useState(null);

  const loadProjects = async () => {
    const response = await fetch(`${apiUrl}/api/projects`);
    if (response.ok) setProjects(await response.json());
  };

  const loadMessages = async () => {
    const response = await fetch(`${apiUrl}/api/contact-messages`, { headers: authHeaders() });
    if (response.ok) setMessages(await response.json());
  };

  const loadCertificates = async () => {
    const response = await fetch(`${apiUrl}/api/certificates`);
    if (response.ok) setCertificates(await response.json());
  };

  useEffect(() => {
    fetch(`${apiUrl}/api/auth/me`, { headers: authHeaders() })
      .then((response) => {
        setAuthenticated(response.ok);
        if (response.ok) loadMessages();
      });
    loadProjects();
    loadCertificates();
  }, []);

  const submitLogin = async (event) => {
    event.preventDefault();
    const response = await fetch(`${apiUrl}/api/auth/login`, {
      method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(login),
    });
    if (response.ok) {
      const result = await response.json();
      sessionStorage.setItem(tokenKey, result.token);
    }
    setAuthenticated(response.ok);
    if (response.ok) loadMessages();
    setStatus(response.ok ? 'Login successful.' : 'Invalid email or password.');
  };

  const saveProject = async (event) => {
    event.preventDefault();
    const response = await fetch(`${apiUrl}/api/projects${editingId ? `/${editingId}` : ''}`, {
      method: editingId ? 'PUT' : 'POST', headers: { 'Content-Type': 'application/json', ...authHeaders() }, body: JSON.stringify(project),
    });
    if (response.ok) {
      setProject(emptyProject);
      setEditingId(null);
      setStatus(editingId ? 'Project updated successfully.' : 'Project added successfully.');
      loadProjects();
    } else setStatus('Unable to save project. Please check all fields.');
  };

  const editProject = (item) => {
    setEditingId(item.id);
    setProject(Object.fromEntries(Object.keys(emptyProject).map((key) => [key, item[key] || ''])));
    setStatus('Editing selected project.');
    document.getElementById('project-form')?.scrollIntoView({ behavior: 'smooth' });
  };

  const cancelEdit = () => {
    setEditingId(null);
    setProject(emptyProject);
    setStatus('');
  };

  const deleteProject = async (id) => {
    if (!window.confirm('Delete this project?')) return;
    const response = await fetch(`${apiUrl}/api/projects/${id}`, { method: 'DELETE', headers: authHeaders() });
    if (response.ok) loadProjects();
  };

  const saveCertificate = async (event) => {
    event.preventDefault();
    setStatus('Saving certificate...');
    let nextCertificate = { ...certificate, issueYear: Number(certificate.issueYear) };

    if (certificateFile) {
      const formData = new FormData();
      formData.append('file', certificateFile);
      const upload = await fetch(`${apiUrl}/api/certificates/upload`, { method: 'POST', headers: authHeaders(), body: formData });
      if (!upload.ok) {
        const error = await upload.json().catch(() => ({}));
        setStatus(error.message || 'Unable to upload certificate file.');
        return;
      }
      nextCertificate = { ...nextCertificate, ...await upload.json() };
    }

    const response = await fetch(`${apiUrl}/api/certificates${editingCertificateId ? `/${editingCertificateId}` : ''}`, {
      method: editingCertificateId ? 'PUT' : 'POST',
      headers: { 'Content-Type': 'application/json', ...authHeaders() },
      body: JSON.stringify(nextCertificate),
    });
    if (response.ok) {
      setCertificate(emptyCertificate);
      setCertificateFile(null);
      setEditingCertificateId(null);
      setStatus(editingCertificateId ? 'Certificate updated successfully.' : 'Certificate added successfully.');
      loadCertificates();
    } else setStatus('Unable to save certificate. Please check all fields.');
  };

  const editCertificate = (item) => {
    setCertificate({ ...emptyCertificate, ...item });
    setEditingCertificateId(item.id);
    setCertificateFile(null);
    setStatus('Editing selected certificate.');
    document.getElementById('certificate-form')?.scrollIntoView({ behavior: 'smooth' });
  };

  const cancelCertificateEdit = () => {
    setCertificate(emptyCertificate);
    setCertificateFile(null);
    setEditingCertificateId(null);
    setStatus('');
  };

  const deleteCertificate = async (id) => {
    if (!window.confirm('Delete this certificate?')) return;
    const response = await fetch(`${apiUrl}/api/certificates/${id}`, { method: 'DELETE', headers: authHeaders() });
    if (response.ok) loadCertificates();
  };

  const toggleMessage = async (item) => {
    const response = await fetch(`${apiUrl}/api/contact-messages/${item.id}/read?value=${!item.isRead}`, {
      method: 'PATCH', headers: authHeaders(),
    });
    if (response.ok) loadMessages();
  };

  const deleteMessage = async (id) => {
    if (!window.confirm('Delete this message?')) return;
    const response = await fetch(`${apiUrl}/api/contact-messages/${id}`, { method: 'DELETE', headers: authHeaders() });
    if (response.ok) loadMessages();
  };

  const logout = async () => {
    await fetch(`${apiUrl}/api/auth/logout`, { method: 'POST', headers: authHeaders() });
    sessionStorage.removeItem(tokenKey);
    setAuthenticated(false);
    setLogin({ email: '', password: '' });
    setMessages([]);
    setStatus('You have been logged out.');
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
    <header className="admin-header"><div><p className="eyebrow">PORTFOLIO ADMIN</p><h1>Dashboard</h1></div><div className="admin-top-actions"><a href="/">View Portfolio</a><button onClick={logout}>Logout</button></div></header>
    <nav className="admin-nav"><a href="#project-form">Add Project</a><a href="#saved-projects">Projects ({projects.length})</a><a href="#certificate-form">Certificates ({certificates.length})</a><a href="#messages">Messages ({messages.length})</a></nav>
    <form id="project-form" className="admin-card admin-form" onSubmit={saveProject}>
      <h2>{editingId ? 'Edit Project' : 'Add New Project'}</h2>
      {Object.keys(emptyProject).map((name) => <label key={name}>{name.replace(/([A-Z])/g, ' $1')}
        {name === 'description' || name === 'features' ? <textarea required={name === 'description'} value={project[name]} onChange={(e) => setProject({ ...project, [name]: e.target.value })} /> : <input type={name.toLowerCase().includes('url') ? 'url' : 'text'} required={['title','technologies'].includes(name)} value={project[name]} onChange={(e) => setProject({ ...project, [name]: e.target.value })} />}
      </label>)}
      {status && <p className="admin-status">{status}</p>}<div className="admin-form-actions"><button className="btn primary">{editingId ? 'Save Changes' : 'Add Project'}</button>{editingId && <button type="button" className="btn secondary" onClick={cancelEdit}>Cancel</button>}</div>
    </form>
    <section id="saved-projects" className="admin-list"><h2>Saved Projects</h2>{projects.length ? projects.map((item) => <article className="admin-project" key={item.id}><div><h3>{item.title}</h3><p>{item.technologies}</p></div><div className="project-actions"><button className="edit" onClick={() => editProject(item)}>Edit</button><button onClick={() => deleteProject(item.id)}>Delete</button></div></article>) : <p>No projects added yet.</p>}</section>
    <form id="certificate-form" className="admin-card admin-form" onSubmit={saveCertificate}>
      <h2>{editingCertificateId ? 'Edit Certificate' : 'Add New Certificate'}</h2>
      <label>Certificate Title<input required minLength="3" value={certificate.title} onChange={(e) => setCertificate({ ...certificate, title: e.target.value })} /></label>
      <label>Issuing Organization<input required minLength="2" value={certificate.issuer} onChange={(e) => setCertificate({ ...certificate, issuer: e.target.value })} /></label>
      <label>Issue Year<input type="number" min="2000" max="2100" required value={certificate.issueYear} onChange={(e) => setCertificate({ ...certificate, issueYear: e.target.value })} /></label>
      <label>Description<textarea value={certificate.description} onChange={(e) => setCertificate({ ...certificate, description: e.target.value })} /></label>
      <label>Certificate File <span>(JPG, PNG, WebP or PDF - max 5 MB)</span><input type="file" accept="image/jpeg,image/png,image/webp,application/pdf" onChange={(e) => setCertificateFile(e.target.files?.[0] || null)} /></label>
      {certificate.fileUrl && <p><a href={certificate.fileUrl} target="_blank" rel="noreferrer">View current certificate file</a></p>}
      {status && <p className="admin-status">{status}</p>}
      <div className="admin-form-actions"><button className="btn primary">{editingCertificateId ? 'Save Changes' : 'Add Certificate'}</button>{editingCertificateId && <button type="button" className="btn secondary" onClick={cancelCertificateEdit}>Cancel</button>}</div>
    </form>
    <section className="admin-list"><h2>Saved Certificates</h2>{certificates.length ? certificates.map((item) => <article className="admin-project" key={item.id}><div><h3>{item.title}</h3><p>{item.issuer} · {item.issueYear}</p></div><div className="project-actions"><button className="edit" onClick={() => editCertificate(item)}>Edit</button><button onClick={() => deleteCertificate(item.id)}>Delete</button></div></article>) : <p>No certificates added yet.</p>}</section>
    <section id="messages" className="admin-list"><h2>Contact Messages</h2>{messages.length ? messages.map((item) => <article className={`admin-message ${item.isRead ? 'read' : ''}`} key={item.id}>
      <div className="message-meta"><span className="message-badge">{item.isRead ? 'Read' : 'New'}</span><time>{new Date(item.createdAt).toLocaleString()}</time></div>
      <h3>{item.subject}</h3><p><strong>{item.name}</strong> · <a href={`mailto:${item.email}`}>{item.email}</a>{item.phone && ` · ${item.phone}`}</p><p>{item.message}</p>
      <div className="message-actions"><button onClick={() => toggleMessage(item)}>Mark as {item.isRead ? 'Unread' : 'Read'}</button><button className="danger" onClick={() => deleteMessage(item.id)}>Delete</button></div>
    </article>) : <p>No contact messages yet.</p>}</section>
  </div></main>;
}
