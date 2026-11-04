export const WIDTH = 400;
export const HEIGHT = 500;
export const SHELF_WIDTH = 80;
export const SHELF_HEIGHT = 12;
export const SHELF_Y = HEIGHT - 30;
export const BOOK_SIZE = 24;

export function newGame() {
    return { shelfX: (WIDTH - SHELF_WIDTH) / 2, books: [], score: 0, lives: 3, over: false };
}

export function moveShelf(shelfX, direction) {
    const x = shelfX + direction * 6;
    return Math.max(0, Math.min(WIDTH - SHELF_WIDTH, x));
}

export function isCaught(book, shelfX) {
    return book.y + BOOK_SIZE >= SHELF_Y
        && book.y <= SHELF_Y + SHELF_HEIGHT
        && book.x + BOOK_SIZE > shelfX
        && book.x < shelfX + SHELF_WIDTH;
}

export function fallSpeed(score) {
    return 2 + Math.floor(score / 50) * 0.5;
}

export function updateBooks(game) {
    const speed = fallSpeed(game.score);
    const left = [];

    for (const book of game.books) {
        book.y += speed;
        if (isCaught(book, game.shelfX)) {
            game.score += 10;
        } else if (book.y > HEIGHT) {
            game.lives -= 1;
        } else {
            left.push(book);
        }
    }

    game.books = left;
    if (game.lives <= 0) {
        game.over = true;
    }
}
