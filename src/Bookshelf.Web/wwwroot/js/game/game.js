import { WIDTH, HEIGHT, SHELF_WIDTH, SHELF_HEIGHT, SHELF_Y, BOOK_SIZE, newGame, moveShelf, updateBooks } from './logic.js';

const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
const colors = ['#c0392b', '#2471a3', '#1e8449', '#b9770e'];

let game = newGame();
let leftPressed = false;
let rightPressed = false;
let frame = 0;

document.addEventListener('keydown', event => {
    if (event.key === 'ArrowLeft') leftPressed = true;
    if (event.key === 'ArrowRight') rightPressed = true;
    if (event.key === ' ' && game.over) game = newGame();
    if (event.key === 'ArrowLeft' || event.key === 'ArrowRight' || event.key === ' ') event.preventDefault();
});

document.addEventListener('keyup', event => {
    if (event.key === 'ArrowLeft') leftPressed = false;
    if (event.key === 'ArrowRight') rightPressed = false;
});

canvas.addEventListener('click', () => {
    if (game.over) game = newGame();
});

function update() {
    if (game.over) return;

    if (leftPressed) game.shelfX = moveShelf(game.shelfX, -1);
    if (rightPressed) game.shelfX = moveShelf(game.shelfX, 1);

    frame++;
    if (frame % 60 === 0) {
        game.books.push({
            x: Math.random() * (WIDTH - BOOK_SIZE),
            y: -BOOK_SIZE,
            color: colors[Math.floor(Math.random() * colors.length)],
        });
    }

    updateBooks(game);
}

function draw() {
    ctx.fillStyle = '#fdf6e3';
    ctx.fillRect(0, 0, WIDTH, HEIGHT);

    for (const book of game.books) {
        ctx.fillStyle = book.color;
        ctx.fillRect(book.x, book.y, BOOK_SIZE, BOOK_SIZE);
    }

    ctx.fillStyle = '#8b5a2b';
    ctx.fillRect(game.shelfX, SHELF_Y, SHELF_WIDTH, SHELF_HEIGHT);

    ctx.fillStyle = '#212529';
    ctx.font = '18px sans-serif';
    ctx.textAlign = 'left';
    ctx.fillText('Очки: ' + game.score, 10, 25);
    ctx.textAlign = 'right';
    ctx.fillText('Життя: ' + game.lives, WIDTH - 10, 25);

    if (game.over) {
        ctx.fillStyle = 'rgba(0, 0, 0, 0.6)';
        ctx.fillRect(0, 0, WIDTH, HEIGHT);
        ctx.fillStyle = '#fff';
        ctx.textAlign = 'center';
        ctx.font = '32px sans-serif';
        ctx.fillText('Гра закінчена', WIDTH / 2, HEIGHT / 2 - 20);
        ctx.font = '16px sans-serif';
        ctx.fillText('Пробіл або клік — почати знову', WIDTH / 2, HEIGHT / 2 + 20);
    }
}

function loop() {
    update();
    draw();
    requestAnimationFrame(loop);
}

loop();
