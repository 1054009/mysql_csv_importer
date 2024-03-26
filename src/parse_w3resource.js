const paragraphs = document.querySelectorAll("p")

let finalText = ""

paragraphs.forEach((p) =>
{
	const strong = p.querySelector("strong")
	if (!strong) return
	if (!(/^[0-9]./).test(strong.innerHTML)) return

	const text = p.innerHTML.replace("<strong>", "").replace("</strong>", "").trim()

	finalText += `-- ${text}\n\n\n`
})

{
	const magical_button = document.createElement("button")

	magical_button.innerHTML = "Click to Copy Awesome Stuff"
	magical_button.m_strText = finalText

	magical_button.style.zIndex = 999
	magical_button.style.position = "absolute"

	magical_button.onclick = (event) =>
	{
		if (!event || !event.target) return

		const button = event.target

		navigator.clipboard.writeText(button.m_strText)

		button.remove()
	}

	magical_button.scrollTo()

	document.body.appendChild(magical_button)
}
